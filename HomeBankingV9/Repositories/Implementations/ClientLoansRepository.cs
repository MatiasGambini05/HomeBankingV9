using HomeBankingV9.Models;

namespace HomeBankingV9.Repositories.Implementations
{
    public class ClientLoansRepository : RepositoryBase<ClientLoan>, IClientLoansRepository
    {
        public ClientLoansRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public void Save(ClientLoan clientLoan)
        {
            Create(clientLoan);
            Savechanges();
        }
    }
}
