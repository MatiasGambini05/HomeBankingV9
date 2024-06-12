using HomeBankingV9.DTOs;
using HomeBankingV9.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingV9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;
/*      private readonly IClientRepository _clientRepository;      
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;*/
        public ClientsController(IClientService clientService, IAccountService accountService,
            ICardService cardService /*, IClientRepository clientRepository, 
            IAccountRepository accountRepository, ICardRepository cardRepository */)
        {
            _clientService = clientService;
            _accountService = accountService;
            _cardService = cardService;
/*          _clientRepository = clientRepository;
 *          _accountRepository = accountRepository;
            _cardRepository = cardRepository;*/
        }

        //MÉTODOS GET
        [HttpGet]
        [Authorize(Policy ="AdminOnly")]
        public IActionResult GetAllClients()
        {
            try
            {
               IEnumerable<ClientDTO> clientsDTO = _clientService.GetAllClientsDTO();

                return StatusCode(200, clientsDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetClientsByID(long id)
        {
            try
            {
                ClientDTO clientDTO = _clientService.GetClientDTOById(id);

                return StatusCode(200, clientDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return StatusCode(403);

                ClientDTO clientDTO = _clientService.GetClientDTOByEmail(email);
                if (clientDTO == null)
                    return StatusCode(403);


                return StatusCode(200, clientDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("current/accounts")]
        public IActionResult GetClientAccounts(long id)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return StatusCode(403);

                ICollection<ClientAccountDTO> clientAccountsDTO = _clientService.GetClientAccounts(email);
                if (clientAccountsDTO == null)
                    return StatusCode(403);

                return StatusCode(200, clientAccountsDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("current/cards")]
        public IActionResult GetClientCards(long id)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return StatusCode(403);

                ICollection<CardDTO> clientCards = _clientService.GetClientCards(email);
                if (clientCards == null)
                    return StatusCode(403);

                return StatusCode(200, clientCards);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        //MÉTODOS POST
        [HttpPost]
        public IActionResult NewClient([FromBody] NewClientDTO newClientDTO)
        {
            try
            {
                var isEmptyField = _clientService.IsEmptyField(newClientDTO);
                if (isEmptyField == false)
                    return StatusCode(403, "Ningún campo puede estar vacío");

                bool emailExist = _clientService.EmailExist(newClientDTO.Email);
                if (emailExist == true)
                    return StatusCode(403, "El Email ya está en uso");

                _clientService.NewClient(newClientDTO);

                return StatusCode(201, "Cliente creado correctamente");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("current/accounts")]
        [Authorize(Policy = "Client Only")]
        public IActionResult NewAccount()
        {
            try
            {
                var clientEmail = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (clientEmail == string.Empty)
                    return StatusCode(403);

                if (_clientService.GetAccountsAmount(clientEmail) >= 3)
                    return StatusCode(403, "El cliente alcanzó el número máximo de cuentas");

                _accountService.NewAccount(clientEmail);

                return StatusCode(201, "Cuenta creada correctamente");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "Client Only")]
        public IActionResult NewCard([FromBody] TypeColorDTO typeColorDTO)
        {
            try
            {
                var clientEmail = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                    if (clientEmail == string.Empty)
                        return StatusCode(403);

                string status = _cardService.NewCard(clientEmail, typeColorDTO);
                    if (status == "Tarjeta creada correctamente.")
                    return StatusCode(201, status);
                else
                    return StatusCode(403, status);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
