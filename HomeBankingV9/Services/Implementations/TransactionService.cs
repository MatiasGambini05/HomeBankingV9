using HomeBankingV9.DTOs;
using HomeBankingV9.Models;
using HomeBankingV9.Repositories;

namespace HomeBankingV9.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ITransactionRepository _transactionRepository;
        public TransactionService(IClientRepository clientRepository, IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public string NewTransaction(string email, TransfDTO transfDTO)
        {
            if (String.IsNullOrEmpty(transfDTO.FromAccountNumber) || String.IsNullOrEmpty(transfDTO.ToAccountNumber) || //Verificar que no haya parámetro vacío.
                String.IsNullOrEmpty(transfDTO.Description) || transfDTO.Amount <= 0)
                return "Ningún campo puede estar vacío.";

            if (transfDTO.FromAccountNumber.Equals(transfDTO.ToAccountNumber)) //Verificar que no sea la misma cuenta de origen y destino.
                return "La cuenta de destino no puede ser la misma que la cuenta de origen.";

            Client client = _clientRepository.FindClientByEmail(email); //Implementarlo con clientservice.
            ClientDTO clientDTO = new ClientDTO(client);

            var allaccounts = _accountRepository.FindAllAccounts();
            var clientAccounts = clientDTO.Accounts;
            if (allaccounts.Any(acc => acc.Number == transfDTO.FromAccountNumber)) //verificar que la cuenta de origen exista
            {
                if (!clientAccounts.Any(acc => acc.Number == transfDTO.FromAccountNumber)) //verificar que la cuenta de origen sea del cliente logeado
                    return "La cuenta de origen no puede ser de otro cliente.";
            }
            else
                return "La cuenta de origen no existe.";

            if (!allaccounts.Any(acc => acc.Number == transfDTO.ToAccountNumber))//verificar si existe cuenta destino
                return "La cuenta destino no existe.";

            var fromAccount = allaccounts.FirstOrDefault(acc => acc.Number == transfDTO.FromAccountNumber);
            var toAccount = allaccounts.FirstOrDefault(acc => acc.Number == transfDTO.ToAccountNumber);
            if (fromAccount.Balance < transfDTO.Amount) //verificar que la cuenta de origen tenga saldo suficiente para realizar la transferencia
                return "La cuenta de origen no tiene saldo suficiente.";

            Transaction transactionDebit = new Transaction
            {
                Type = TransactionType.DEBIT,
                Amount = transfDTO.Amount,
                Description = transfDTO.Description + " - Transferencia realizada hacia cuenta " + toAccount.Number + ".",
                Date = DateTime.Now,
                AccountId = fromAccount.Id
            };
            _transactionRepository.Save(transactionDebit);

            Transaction transactionCredit = new Transaction
            {
                Type = TransactionType.CREDIT,
                Amount = transfDTO.Amount,
                Description = transfDTO.Description + " - Transferencia recibida desde cuenta " + fromAccount.Number + ".",
                Date = DateTime.Now,
                AccountId = toAccount.Id
            };
            _transactionRepository.Save(transactionCredit);

            fromAccount.Balance = fromAccount.Balance - transfDTO.Amount;
            toAccount.Balance = toAccount.Balance + transfDTO.Amount;
            _accountRepository.Save(fromAccount);
            _accountRepository.Save(toAccount);

            return "Transferencia realizada correctamente.";
        }
    }
}