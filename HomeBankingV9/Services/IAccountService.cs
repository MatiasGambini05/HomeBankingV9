using HomeBankingV9.DTOs;
using HomeBankingV9.Models;

namespace HomeBankingV9.Services
{
    public interface IAccountService
    {
        IEnumerable<AccountDTO> GetAllAccounts();
        AccountDTO GetAccountById(long id);
        void NewAccount(string email);
        void Save(Account account);
    }
}
