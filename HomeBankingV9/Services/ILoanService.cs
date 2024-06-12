using HomeBankingV9.DTOs;
using HomeBankingV9.Models;

namespace HomeBankingV9.Services
{
    public interface ILoanService
    {
        IEnumerable<Loan> GetAllLoans();
        string NewLoan(string email, NewLoanDTO newLoanDTO);
    }
}
