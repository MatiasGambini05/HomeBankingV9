using HomeBankingV9.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingV9.Repositories.Implementations
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Account> FindAllAccounts()
        {
            return FindAll()
            .Include(account => account.Transactions)
            .ToList();
        }

        public Account FindAccountById(long id)
        {
            return FindByCondition(account => account.Id == id)
            .Include(account => account.Transactions)
            .FirstOrDefault();
        }

        public Account FindAccountByNumber(string number)
        {
            return FindByCondition(account => account.Number.ToUpper() == number.ToUpper())
            .Include(account => account.Transactions)
            .FirstOrDefault();
        }

        public IEnumerable<Account> FindAccountsByClient(long clientId)

        {
            return FindByCondition(account => account.ClientId == clientId)
            .Include(account => account.Transactions)
            .ToList();
        }

        public void Save(Account account)
        {
            if (account.Id == 0)
                Create(account);
            else
                Update(account);

            Savechanges();
            RepositoryContext.ChangeTracker.Clear();
        }
    }
}
