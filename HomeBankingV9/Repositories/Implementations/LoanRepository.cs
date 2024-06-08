using HomeBankingV9.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingV9.Repositories.Implementations
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Loan> FindAllLoans()
        {
            return FindAll();
        }

        public Loan FindLoanById(long id)
        {
            return FindByCondition(l => l.Id == id)
                .FirstOrDefault();
        }
    }
}
