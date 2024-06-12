using HomeBankingV9.DTOs;
using HomeBankingV9.Models;
using HomeBankingV9.Repositories;
using HomeBankingV9.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingV9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanService _loanService;
        private readonly IClientLoansRepository _clientLoansRepository;
        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository,
              ILoanRepository loanRepository, ILoanService loanService, IClientLoansRepository clientLoansRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _loanRepository = loanRepository;
            _loanService = loanService;
            _clientLoansRepository = clientLoansRepository;
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
                if (String.IsNullOrEmpty(newLoanDTO.LoanId.ToString()) || String.IsNullOrEmpty(newLoanDTO.ToAccountNumber) || //Verificar que ningún campo esté vacío.
                    String.IsNullOrEmpty(newLoanDTO.Payments) || newLoanDTO.Amount <= 0)
                    return StatusCode(403, "Ningún campo puede estar vacío.");

                var loans = _loanRepository.FindAllLoans();
                if (!loans.Any(l => l.Id == newLoanDTO.LoanId)) //Verificar que el ID del prestamo exista.
                    return StatusCode(403, "El ID del préstamo no existe.");

                var requiredLoan = _loanRepository.FindLoanById(newLoanDTO.LoanId);
                if (newLoanDTO.Amount > requiredLoan.MaxAmount) //Verificar que el monto solicitado no supere al máximo del prestamo.
                    return StatusCode(403, "El monto solicitado excede al máximo del préstamo.");

                var paymentList = requiredLoan.Payments.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(payment => int.Parse(payment.Trim()))
                            .ToList();
                var paymentToInt = int.Parse(newLoanDTO.Payments);

                if (!paymentList.Contains(paymentToInt)) //Verificar que la cantidad de cuotas solicitada coincida con las que brinda el prestamo.
                    return StatusCode(403, "La cantidad de cuotas solicitada no es válida para este préstamo.");

                var clientEmail = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                Client client = _clientRepository.FindClientByEmail(clientEmail);
                ClientDTO clientDTO = new ClientDTO(client);
                var allAccounts = _accountRepository.FindAllAccounts();
                var clientAccounts = clientDTO.Accounts;
                var loanAccount = clientAccounts.FirstOrDefault(a => a.Number == newLoanDTO.ToAccountNumber);
                
                if (allAccounts.Any(a => loanAccount.Number == newLoanDTO.ToAccountNumber)) //Verificar que la cuenta de destino exista.
                {
                    if (!clientAccounts.Any(a => a.Number == newLoanDTO.ToAccountNumber)) //Verificar que la cuenta destino pertenezca al cliente.
                        return StatusCode(403, "La cuenta destino no pertenece a este cliente.");
                } else
                    return StatusCode(403, "La cuenta destino no existe.");

                var loanAmount = newLoanDTO.Amount * 1.2;
                ClientLoan clientLoan = new ClientLoan //Crear prestamo.
                {
                    Amount = loanAmount,
                    Payments = newLoanDTO.Payments,
                    ClientId = client.Id,
                    LoanId = newLoanDTO.LoanId,
                };
                _clientLoansRepository.Save(clientLoan);

                Transaction loanDetail = new Transaction //Crear transacción.
                {
                    Type = TransactionType.CREDIT,
                    Amount = newLoanDTO.Amount,
                    Description = requiredLoan.Name + " aprobado en " + newLoanDTO.Payments + " cuotas.",
                    Date = DateTime.Now,
                    AccountId = loanAccount.Id
                };
                _transactionRepository.Save(loanDetail);

                var fromAccount = allAccounts.FirstOrDefault(acc => acc.Number == newLoanDTO.ToAccountNumber);
                fromAccount.Balance = fromAccount.Balance + newLoanDTO.Amount;
                _accountRepository.Save(fromAccount); //Cambiar balance en cuenta.

                return StatusCode(201, "Prestamo solicitado correctamente.");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
