using HomeBankingV9.Models;

namespace HomeBankingV9.Repositories.Implementations
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Transaction> FindAllTransactions()
        {
            return FindAll()
                .ToList();
        }

        public Transaction FindTransactionById(long id)
        {
            return FindByCondition(t => t.Id == id)
                .FirstOrDefault();
        }

        public void Save(Transaction transaction)
        {
            Create(transaction);
            Savechanges();
        }
    }
}
