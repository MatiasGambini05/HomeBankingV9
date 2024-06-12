using HomeBankingV9.Models;
using HomeBankingV9.Repositories;
using HomeBankingV9.Repositories.Implementations;
using System.Collections.Generic;

namespace HomeBankingV9.Services.Implementations
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        public LoanService(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public IEnumerable<Loan> GetAllLoans()
        {
            IEnumerable<Loan> loans = _loanRepository.FindAllLoans();

            return loans;
        }
    }
}
