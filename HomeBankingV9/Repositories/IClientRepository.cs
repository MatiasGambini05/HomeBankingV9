using HomeBankingV9.Models;

namespace HomeBankingV9.Repositories
{
    public interface IClientRepository
    {
        IEnumerable<Client> FindAllClients();
        Client FindClientById(long id);
        Client FindClientByEmail(string email);
        void Save(Client client);
    }
}
