using HomeBankingV9.Models;

namespace HomeBankingV9.DTOs
{
    public class EmailPasswordDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public EmailPasswordDTO(Client client)
        {
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
            Password = client.Password;
        }
    }
}
