using HomeBankingV9.DTOs;
using HomeBankingV9.Models;
using HomeBankingV9.Repositories;
using HomeBankingV9.Repositories.Implementations;
using HomeBankingV9.Services;
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
        private readonly IClientRepository _clientRepository;
        private readonly IClientService _clientService;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        public TransactionsController(IClientRepository clientRepository, IAccountRepository accountRepository,
            ITransactionRepository transactionRepository, IClientService clientService)
        {
            _clientRepository = clientRepository;
            _clientService = clientService;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        [Authorize(Policy = "Client Only")]
        public IActionResult NewTransfer([FromBody] TransfDTO transfDTO)
        {
            try
            {
                if (String.IsNullOrEmpty(transfDTO.FromAccountNumber) || String.IsNullOrEmpty(transfDTO.ToAccountNumber) || //Verificar que no haya parámetro vacío.
                    String.IsNullOrEmpty(transfDTO.Description) || transfDTO.Amount <= 0)
                    return StatusCode(403, "Ningún campo puede estar vacío");

                if (transfDTO.FromAccountNumber.Equals(transfDTO.ToAccountNumber)) //Verificar que no sea la misma cuenta de origen y destino.
                    return StatusCode(403, "La cuenta de destino no puede ser la misma que la cuenta de origen.");

                var clientEmail = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                Client client = _clientRepository.FindClientByEmail(clientEmail);
                ClientDTO clientDTO = new ClientDTO(client);

                var allaccounts = _accountRepository.FindAllAccounts();
                var clientAccounts = clientDTO.Accounts;
                if (allaccounts.Any(acc => acc.Number == transfDTO.FromAccountNumber)) //verificar que la cuenta de origen exista
                {
                    if (!clientAccounts.Any(acc => acc.Number == transfDTO.FromAccountNumber)) //verificar que la cuenta de origen sea del cliente logeado
                        return StatusCode(403, "La cuenta de origen no puede ser de otro cliente.");
                }   else
                    return StatusCode(403, "La cuenta de origen no existe.");

                if (!allaccounts.Any(acc => acc.Number == transfDTO.ToAccountNumber))//verificar si existe cuenta destino
                    return StatusCode(403, "La cuenta destino no existe.");

                var fromAccount = allaccounts.FirstOrDefault(acc => acc.Number == transfDTO.FromAccountNumber);
                var toAccount = allaccounts.FirstOrDefault(acc => acc.Number == transfDTO.ToAccountNumber);
                var fromBalance = fromAccount.Balance;
                if (fromBalance < transfDTO.Amount) //verificar que la cuenta de origen tenga saldo suficiente para realizar la transferencia
                    return StatusCode(403, "La cuenta de origen no tiene saldo suficiente.");

                var toBalance = toAccount.Balance; //Agregar balance en cuenta destino y restar en cuenta origen
                var newFromBalance = fromAccount.Balance - transfDTO.Amount;
                var newToBalance = toAccount.Balance + transfDTO.Amount;
                fromAccount.Balance = newFromBalance;
                _accountRepository.Save(fromAccount);
                toAccount.Balance = newToBalance;
                _accountRepository.Save(toAccount);

                var accountNumberFrom = fromAccount.Number;
                var accountNumberTo = toAccount.Number;
                
                Transaction transactionDebit = new Transaction
                {
                    Type = TransactionType.DEBIT,
                    Amount = transfDTO.Amount,
                    Description = transfDTO.Description + " - Transferencia realizada hacia cuenta " + accountNumberTo + ".",
                    Date = DateTime.Now,
                    AccountId = fromAccount.Id
                };
                _transactionRepository.Save(transactionDebit);

                Transaction transactionCredit = new Transaction
                {
                    Type = TransactionType.CREDIT,
                    Amount = transfDTO.Amount,
                    Description = transfDTO.Description + " - Transferencia recibida desde cuenta " + accountNumberFrom + ".",
                    Date = DateTime.Now,
                    AccountId = toAccount.Id
                };
                _transactionRepository.Save(transactionCredit);

                return StatusCode(201, "Transferencia realizada correctamente.");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
