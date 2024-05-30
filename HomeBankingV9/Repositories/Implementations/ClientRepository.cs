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
                .Include(c => c.Accounts)
                .ToList();
        }

        public Client FindClientById(long id)
        {
            return FindByCondition(c => c.Id == id)
                .Include(c => c.Accounts)
                .FirstOrDefault();
        }

        public void Save(Client client)
        {
            Create(client);
            Savechanges();
        }
    }
}
