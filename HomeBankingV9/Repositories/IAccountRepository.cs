using HomeBankingV9.Models;

namespace HomeBankingV9.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> FindAllAccounts();
        Account FindAccountById(long id);
        void Save(Account account);
    }
}
