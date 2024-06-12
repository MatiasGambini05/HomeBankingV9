using HomeBankingV9.DTOs;
using HomeBankingV9.Models;
using HomeBankingV9.Repositories;
using HomeBankingV9.Services;
using HomeBankingV9.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingV9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
/*      private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionService _transactionService;
        private readonly ILoanRepository _loanRepository;
        private readonly IClientLoansRepository _clientLoansRepository;*/
        public LoansController(ILoanService loanService /*,IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository,
              ILoanRepository loanRepository, IClientLoansRepository clientLoansRepository,
              ITransactionService transactionService*/)
        {
            _loanService = loanService;
 /*         _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _transactionService = transactionService;
            _loanRepository = loanRepository;
            _clientLoansRepository = clientLoansRepository;
            _transactionService = transactionService;*/
        }

        [HttpGet]
        [Authorize(Policy = "Client Only")]
        public IActionResult GetAllLoans()
        {
            try
            {
                var loans = _loanService.GetAllLoans();
                return StatusCode(200, loans);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "Client Only")]
        public IActionResult NewLoan([FromBody] NewLoanDTO newLoanDTO)
        {
            try
            {
                var clientEmail = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (clientEmail == string.Empty)
                    return StatusCode(403);

                string status = _loanService.NewLoan(clientEmail, newLoanDTO);
                    if (status == "Prestamo aprobado correctamente.")
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
