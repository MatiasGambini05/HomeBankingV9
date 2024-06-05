using HomeBankingV9.DTOs;
using HomeBankingV9.Models;
using HomeBankingV9.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HomeBankingV9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        [Authorize(Policy ="AdminOnly")]
        public IActionResult GetAllClients()
        {
            try
            {
                var clients = _clientRepository.FindAllClients();
                var clientsDTO = clients.Select(c => new ClientDTO(c)).ToList();
                return StatusCode(StatusCodes.Status200OK, clientsDTO);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetClientsByID(long id)
        {
            try
            {
                var client = _clientRepository.FindClientById(id);
                var clientDTO = new ClientDTO(client);
                return StatusCode(StatusCodes.Status200OK, clientDTO);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return Forbid();

                Client client = _clientRepository.FindClientByEmail(email);
                if (client == null)
                    return Forbid();

                var clientDTO = new ClientDTO(client);
                return Ok(clientDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] NewClientDTO newClientDTO)
        {
            try
            {
                if (String.IsNullOrEmpty(newClientDTO.Email) || String.IsNullOrEmpty(newClientDTO.Password) ||
                    String.IsNullOrEmpty(newClientDTO.FirstName) || String.IsNullOrEmpty(newClientDTO.LastName))
                    return StatusCode(403, "Datos Inválidos");

                Client user = _clientRepository.FindClientByEmail(newClientDTO.Email);

                if (user != null)
                    return StatusCode(403, "El Email ya está en uso");

                Client newClient = new Client
                {
                    Email = newClientDTO.Email,
                    Password = newClientDTO.Password,
                    FirstName = newClientDTO.FirstName,
                    LastName = newClientDTO.LastName,
                };

                _clientRepository.Save(newClient);
                return StatusCode(201, "Cliente creado correctamente");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
