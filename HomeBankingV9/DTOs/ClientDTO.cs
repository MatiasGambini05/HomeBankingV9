using HomeBankingV9.Models;

namespace HomeBankingV9.DTOs
{
    public class ClientDTO
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public ICollection<ClientAccountDTO> Accounts { get; set; }
        public ICollection<ClientLoanDTO> Loans { get; set; }
        public ICollection<CardDTO> Cards { get; set; }

        public ClientDTO(Client client)
        {
            Id = client.Id;
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
            Accounts = client.Accounts.Select(a => new ClientAccountDTO(a)).ToList();
            Loans = client.Loans.Select(cl => new ClientLoanDTO(cl)).ToList();
            Cards = client.Cards.Select(ca => new CardDTO(ca)).ToList();
        }
    }
}
