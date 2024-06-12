using HomeBankingV9.DTOs;
using HomeBankingV9.Models;

namespace HomeBankingV9.Services
{
    public interface IClientService
    {
        IEnumerable<ClientDTO> GetAllClientsDTO();
        ClientDTO GetClientDTOById(long id);
        ClientDTO GetClientDTOByEmail(string email);
        ICollection<ClientAccountDTO> GetClientAccounts(string email);
        ICollection<CardDTO> GetClientCards(string email);
        bool IsEmptyField(NewClientDTO newClientDTO);
        void NewClient(NewClientDTO newClientDTO);
        bool EmailExist (string email);
        int GetAccountsAmount(string email);
    }
}
