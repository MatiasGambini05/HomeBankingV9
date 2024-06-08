using HomeBankingV9.Models;

namespace HomeBankingV9.Repositories
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> FindAllLoans();
        Loan FindLoanById(long id);
    }
}
