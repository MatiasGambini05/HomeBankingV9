using HomeBankingV9.DTOs;
using HomeBankingV9.Models;
using HomeBankingV9.Repositories;
using HomeBankingV9.Repositories.Implementations;
using HomeBankingV9.Services;
using HomeBankingV9.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace HomeBankingV9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
/*      private readonly IClientRepository _clientRepository;
        private readonly IClientService _clientService;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;*/
        public TransactionsController(ITransactionService transactionService /*,IClientRepository clientRepository, IAccountRepository accountRepository,
            ITransactionRepository transactionRepository, IClientService clientService*/)
        {
            _transactionService = transactionService;
            /*_clientRepository = clientRepository;
            _clientService = clientService;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;*/
        }

        [HttpPost]
        [Authorize(Policy = "Client Only")]
        public IActionResult NewTransfer([FromBody] TransfDTO transfDTO)
        {
            try
            {
                var clientEmail = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (clientEmail == string.Empty)
                    return StatusCode(403);

                string status = _transactionService.NewTransaction(clientEmail, transfDTO);
                if (status == "Transferencia realizada correctamente.")
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
