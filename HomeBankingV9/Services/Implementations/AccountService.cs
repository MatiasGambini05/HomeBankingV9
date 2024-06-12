using HomeBankingV9.DTOs;
using HomeBankingV9.Models;
using HomeBankingV9.Repositories;

namespace HomeBankingV9.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IClientRepository _clientRepository;
        public AccountService(IAccountRepository accountRepository, IClientRepository clientRepository)
        { 
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
        }

        public IEnumerable<AccountDTO> GetAllAccounts()
        {
            var accounts = _accountRepository.FindAllAccounts();
            var accountsDTO = accounts.Select(a => new AccountDTO(a)).ToList();

            return accountsDTO;
        }
        public AccountDTO GetAccountById(long id)
        {
            var account = _accountRepository.FindAccountById(id);
            var accountDTO = new AccountDTO(account);

            return accountDTO;
        }

        public void NewAccount(string email)
        {
            Client client = _clientRepository.FindClientByEmail(email);
            ClientDTO clientDTO = new ClientDTO(client);
            
            Random random = new Random();
            int randomNumber = random.Next(0, 99999999);
            string randomAccount = "VIN-" + randomNumber.ToString("D8");

            var allAccounts = _accountRepository.FindAllAccounts();
            while (allAccounts.Any(acc => acc.Number == randomAccount))
            {
                int randomNumber2 = random.Next(0, 999999);
                randomAccount = "VIN-" + randomNumber2.ToString("D8");
            }

            Account newAccount = new Account
            {
                Number = randomAccount,
                CreationDate = DateTime.Now,
                Balance = 0,
                ClientId = clientDTO.Id
            };
            _accountRepository.Save(newAccount);
        }

        public void Save(Account account)
        {
            _accountRepository.Save(account);
        }
    }
}
