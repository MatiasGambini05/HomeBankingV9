using HomeBankingV9.Models;

namespace HomeBankingV9.Services
{
    public interface ILoanService
    {
        IEnumerable<Loan> GetAllLoans();
    }
}
