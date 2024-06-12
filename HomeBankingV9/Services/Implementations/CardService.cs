using HomeBankingV9.DTOs;
using HomeBankingV9.Models;
using HomeBankingV9.Repositories;

namespace HomeBankingV9.Services.Implementations
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IClientRepository _clientRepository;
        public CardService(ICardRepository cardRepository, IClientRepository clientRepository)
        {
            _cardRepository = cardRepository;
            _clientRepository = clientRepository;
        }

        public string NewCard(string email, TypeColorDTO typeColorDTO)
        {
            Client client = _clientRepository.FindClientByEmail(email);
            ClientDTO clientDTO = new ClientDTO(client);
            var clientCards = clientDTO.Cards;

            if (clientCards.Count() >= 6)
                return "Alcanzaste el máximo de tarjetas.";


            var debitCards = clientCards.Where(card => card.Type == "DEBIT").ToList();
            var creditCards = clientCards.Where(card => card.Type == "CREDIT").ToList();

            var countDebit = debitCards.Count();
            var countCredit = creditCards.Count();

            if (countCredit >= 3 && typeColorDTO.Type == "CREDIT")
                return "Alcanzaste el máximo de tarjetas de crédito.";

            if (countDebit >= 3 && typeColorDTO.Type == "DEBIT")
                return "Alcanzaste el máximo de tarjetas de débito.";

            var cardInfo = clientCards.Select(card => new { card.Type, card.Color }).ToList();
            if (cardInfo.Any(card => card.Type == typeColorDTO.Type && card.Color == typeColorDTO.Color))
                return "La tarjeta ya está creada.";

            var typeToInt = (CardType)Enum.Parse(typeof(CardType), typeColorDTO.Type, true);
            var colorToInt = (CardColor)Enum.Parse(typeof(CardColor), typeColorDTO.Color, true);

            Card verif = new Card();
            string cardNumber = "";
            do
            {
                Random randomCard = new Random();
                cardNumber = "";
                for (int i = 0; i < 4; i++)
                {
                    int randomCardNumber = randomCard.Next(0, 10000);
                    cardNumber = cardNumber + randomCardNumber.ToString("D4") + "-";
                }
                cardNumber = cardNumber.Remove(cardNumber.Length - 1);
                verif = _cardRepository.FindCardByNumber(cardNumber);
            } while (verif != null);

            Random randomCvv = new Random();
            int randomCvvNumber = randomCvv.Next(100, 1000);

            Card newCard = new Card
            {
                CardHolder = clientDTO.FirstName + " " + clientDTO.LastName,
                Type = typeToInt,
                Color = colorToInt,
                Number = cardNumber,
                Cvv = randomCvvNumber,
                FromDate = DateTime.Now,
                ThruDate = DateTime.Now.AddYears(5),
                ClientId = clientDTO.Id
            };
            _cardRepository.Save(newCard);
            return "Tarjeta creada correctamente.";
        }
    }
}
