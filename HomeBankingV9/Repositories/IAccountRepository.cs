using HomeBankingV9.Models;

namespace HomeBankingV9.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> FindAllAccounts();
        Account FindAccountById(long id);
        Account FindAccountByNumber(string number);
        IEnumerable<Account> FindAccountsByClient(long clientId);
        void Save(Account account);
    }
}
