using HomeBankingV9.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingV9.Repositories.Implementations
{
    public class ClientRepository : RepositoryBase<Client>, IClientRepository
    {
        public ClientRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Client> FindAllClients()
        {
            return FindAll()
                .Include(a => a.Accounts)
                .Include(ca => ca.Cards)
                .Include(cl => cl.Loans)
                    .ThenInclude(l => l.Loan)
                .ToList();
        }

        public Client FindClientById(long id)
        {
            return FindByCondition(c => c.Id == id)
                .Include(a => a.Accounts)
                .Include(ca => ca.Cards)
                .Include(cl => cl.Loans)
                    .ThenInclude(l => l.Loan)
                .FirstOrDefault();
        }

        public Client FindClientByEmail(string email)
        {
            Client client = FindByCondition(c => c.Email.ToUpper() == email.ToUpper())
            .Include(a => a.Accounts)
            .Include(ca => ca.Cards)
            .Include(cl => cl.Loans)
                .ThenInclude(l => l.Loan)
            .FirstOrDefault();

            return client;
        }

        public void Save(Client client)
        {
            Create(client);
            Savechanges();
        }
    }
}
