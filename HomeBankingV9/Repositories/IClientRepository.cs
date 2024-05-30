using HomeBankingV9.Models;

namespace HomeBankingV9.Repositories
{
    public interface IClientRepository
    {
        IEnumerable<Client> FindAllClients();
        Client FindClientById(long id);
        void Save(Client client);
    }
}
