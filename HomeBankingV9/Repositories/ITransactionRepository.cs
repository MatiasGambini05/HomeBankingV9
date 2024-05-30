using HomeBankingV9.Models;

namespace HomeBankingV9.Repositories
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> FindAllTransactions();
        Transaction FindTransactionById(long id);
        void Save(Transaction transaction);
    }
}
