using HomeBankingV9.Models;
using HomeBankingV9.Repositories.Implementations;
using HomeBankingV9.Repositories;
using HomeBankingV9.DTOs;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace HomeBankingV9.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;

        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
        }

        public IEnumerable<ClientDTO> GetAllClientsDTO()
        {
            var clients = _clientRepository.FindAllClients();
            var clientsDTO = clients.Select(c => new ClientDTO(c)).ToList();

            return clientsDTO;
        }

        public ClientDTO GetClientDTOById(long id)
        {
            var client = _clientRepository.FindClientById(id);
            var clientDTO = new ClientDTO(client);

            return clientDTO;
        }

        public ClientDTO GetClientDTOByEmail(string email)
        {
            Client client = _clientRepository.FindClientByEmail(email);
            ClientDTO clientDTO = new ClientDTO(client);

            return clientDTO;
        }

        public ICollection<ClientAccountDTO> GetClientAccounts(string email)
        {
            ClientDTO clientDTO = GetClientDTOByEmail(email);
            var clientAccounts = clientDTO.Accounts;

            return clientAccounts;
        }

        public ICollection<CardDTO> GetClientCards(string email)
        {
            ClientDTO clientDTO = GetClientDTOByEmail(email);
            var clientCards= clientDTO.Cards;

            return clientCards;
        }

        public bool IsEmptyField(NewClientDTO newClientDTO)
        {
            if (String.IsNullOrEmpty(newClientDTO.Email) || String.IsNullOrEmpty(newClientDTO.Password) ||
               String.IsNullOrEmpty(newClientDTO.FirstName) || String.IsNullOrEmpty(newClientDTO.LastName))
                return false;

            return true;
        }

        public void NewClient(NewClientDTO newClientDTO)
        {
            Client newClient = new Client
            {
                Email = newClientDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(newClientDTO.Password),
                FirstName = newClientDTO.FirstName,
                LastName = newClientDTO.LastName
            };
            _clientRepository.Save(newClient);
        }

        public bool EmailExist(string email)
        {
            Client client = _clientRepository.FindClientByEmail(email);
            if (client == null)
                return false;

            return true;
        }

        public int GetAccountsAmount(string email)
        {
            Client client = _clientRepository.FindClientByEmail(email);
            ClientDTO clientDTO = new ClientDTO(client);

            var clientAccounts = clientDTO.Accounts;
            int accountsAmount = clientAccounts.Count();

            return accountsAmount;
        }
    }
}
