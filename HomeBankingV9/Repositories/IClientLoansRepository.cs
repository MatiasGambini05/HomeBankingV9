using HomeBankingV9.Models;

namespace HomeBankingV9.Repositories
{
    public interface IClientLoansRepository
    {
        void Save(ClientLoan clientLoan);
    }
}
