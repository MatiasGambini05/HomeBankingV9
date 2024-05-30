using HomeBankingV9.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
            .Include(a => a.Transactions)
            .ToList();
        }

        public Account FindAccountById(long id)
        {
            return FindByCondition(a => a.Id == id)
            .Include(a => a.Transactions)
            .FirstOrDefault();
        }

        public void Save(Account account)
        {
            Create(account);
            Savechanges();
        }

    }
}
