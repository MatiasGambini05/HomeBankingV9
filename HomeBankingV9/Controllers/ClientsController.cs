using HomeBankingV9.DTOs;
using HomeBankingV9.Models;
using HomeBankingV9.Repositories;
using HomeBankingV9.Repositories.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Principal;

namespace HomeBankingV9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;
        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        //MÉTODOS GET
        [HttpGet]
        [Authorize(Policy ="AdminOnly")]
        public IActionResult GetAllClients()
        {
            try
            {
                var clients = _clientRepository.FindAllClients();
                var clientsDTO = clients.Select(c => new ClientDTO(c)).ToList();
                return StatusCode(StatusCodes.Status200OK, clientsDTO);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetClientsByID(long id)
        {
            try
            {
                var client = _clientRepository.FindClientById(id);
                var clientDTO = new ClientDTO(client);
                return StatusCode(StatusCodes.Status200OK, clientDTO);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return Forbid();

                Client client = _clientRepository.FindClientByEmail(email);
                if (client == null)
                    return Forbid();

                var clientDTO = new ClientDTO(client);
                return Ok(clientDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("current/accounts")]
        public IActionResult GetClientAccounts(long id)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return Forbid();

                Client client = _clientRepository.FindClientByEmail(email);
                ClientDTO clientDTO = new ClientDTO(client);
                if (client == null)
                    return Forbid();

                var clientAccounts = clientDTO.Accounts;

                return Ok(clientAccounts);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("current/cards")]
        public IActionResult GetClientCards(long id)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return Forbid();

                Client client = _clientRepository.FindClientByEmail(email);
                if (client == null)
                    return Forbid();

                ClientDTO clientDTO = new ClientDTO(client);
                var clientCards = clientDTO.Cards;

                return Ok(clientCards);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        //MÉTODOS POST
        [HttpPost]
        public IActionResult NewClient([FromBody] NewClientDTO newClientDTO)
        {
            try
            {
                if (String.IsNullOrEmpty(newClientDTO.Email) || String.IsNullOrEmpty(newClientDTO.Password) ||
                    String.IsNullOrEmpty(newClientDTO.FirstName) || String.IsNullOrEmpty(newClientDTO.LastName))
                    return StatusCode(403, "Ningún campo puede estar vacío");

                Client user = _clientRepository.FindClientByEmail(newClientDTO.Email);

                if (user != null)
                    return StatusCode(403, "El Email ya está en uso");

                Client newClient = new Client
                {
                    Email = newClientDTO.Email,
                    Password = newClientDTO.Password,
                    FirstName = newClientDTO.FirstName,
                    LastName = newClientDTO.LastName,
                };

                _clientRepository.Save(newClient);
                return StatusCode(201, "Cliente creado correctamente");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("current/accounts")]
        [Authorize(Policy = "Client Only")]
        public IActionResult NewAccount()
        {
            try
            {
                var clientEmail = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                Client client= _clientRepository.FindClientByEmail(clientEmail);
                ClientDTO clientDTO = new ClientDTO(client);
                var clientId = clientDTO.Id;
                var clientAccounts = clientDTO.Accounts;
                var allAccounts = _accountRepository.FindAllAccounts();

                if (clientAccounts.Count() >= 3)
                    return StatusCode(403, "El cliente alcanzó el número máximo de cuentas");

                Random random = new Random();
                int randomNumber = random.Next(0, 99999999);
                string randomAccount = "VIN-" + randomNumber.ToString("D8");

                while (allAccounts.Any(acc => acc.Number == randomAccount))
                {
                    int randomNumber2 = random.Next(0, 999999);
                    randomAccount = "VIN" + randomNumber2.ToString("D8");
                }

                Account newAccount = new Account
                {
                    Number = randomAccount,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = clientId
                };
                _accountRepository.Save(newAccount);
                return StatusCode(201, "Cuenta creada correctamente");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "Client Only")]
        public IActionResult NewCard([FromBody] TypeColorDTO typeColorDTO)
        {
            try
            {
                var clientEmail = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                Client client = _clientRepository.FindClientByEmail(clientEmail);
                ClientDTO clientDTO = new ClientDTO(client);
                var clientCards = clientDTO.Cards;

                if (clientCards.Count() >= 6)
                    return StatusCode(403, "Alcanzaste el máximo de tarjetas");


                var debitCards = clientCards.Where(card => card.Type == "DEBIT").ToList();
                var creditCards = clientCards.Where(card => card.Type == "CREDIT").ToList();

                var countDebit = debitCards.Count();
                var countCredit = creditCards.Count();

                if(countCredit >= 3 && typeColorDTO.Type == "CREDIT")
                    return StatusCode(403, "Alcanzaste el máximo de tarjetas de crédito");

                if (countDebit >= 3 && typeColorDTO.Type == "DEBIT")
                    return StatusCode(403, "Alcanzaste el máximo de tarjetas de débito");

                var cardInfo = clientCards.Select(card => new { card.Type, card.Color }).ToList();
                if (cardInfo.Any(card => card.Type == typeColorDTO.Type && card.Color == typeColorDTO.Color))
                    return StatusCode(403, "La tarjeta ya está creada");

                var typeToInt = (CardType)Enum.Parse(typeof(CardType), typeColorDTO.Type, true);
                var colorToInt = (CardColor)Enum.Parse(typeof(CardColor), typeColorDTO.Color, true);

                string cardNumber = "";
                Random randomCard = new Random();
                for (int i=0; i < 4; i++)
                {
                    int randomCardNumber = randomCard.Next(0, 10000);
                    cardNumber = cardNumber + randomCardNumber.ToString("D4") + "-";
                }
                cardNumber = cardNumber.Remove(cardNumber.Length - 1);
                Card verif = _cardRepository.FindCardByNumber(cardNumber);

                if (verif != null)
                {
                    do
                    {
                        cardNumber = "";
                        for (int i = 0; i < 4; i++)
                        {
                            int randomCardNumber = randomCard.Next(0, 10000);
                            cardNumber = cardNumber + randomCardNumber.ToString("D4") + "-";
                        }
                        cardNumber = cardNumber.Remove(cardNumber.Length - 1);
                        verif = _cardRepository.FindCardByNumber(cardNumber);
                    } while (verif != null);
                }

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
                return StatusCode(201, "Tarjeta creada correctamente");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
