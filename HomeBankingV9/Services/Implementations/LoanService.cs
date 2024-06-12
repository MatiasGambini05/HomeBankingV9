using HomeBankingV9.DTOs;
using HomeBankingV9.Models;
using HomeBankingV9.Repositories;
using HomeBankingV9.Repositories.Implementations;
using System.Collections.Generic;
using System.Data;

namespace HomeBankingV9.Services.Implementations
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IClientLoansRepository _clientLoansRepository;
        private readonly ITransactionRepository _transactionRepository;
        public LoanService(ILoanRepository loanRepository, IClientRepository clientRepository,
            IAccountRepository accountRepository, IClientLoansRepository clientLoansRepository,
            ITransactionRepository transactionRepository)
        {
            _loanRepository = loanRepository;
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _clientLoansRepository = clientLoansRepository;
            _transactionRepository = transactionRepository;
        }

        public IEnumerable<Loan> GetAllLoans()
        {
            IEnumerable<Loan> loans = _loanRepository.FindAllLoans();

            return loans;
        }

        public string NewLoan(string email, NewLoanDTO newLoanDTO)
        {
            if (String.IsNullOrEmpty(newLoanDTO.LoanId.ToString()) || String.IsNullOrEmpty(newLoanDTO.ToAccountNumber) || //Verificar que ningún campo esté vacío.
                    String.IsNullOrEmpty(newLoanDTO.Payments) || newLoanDTO.Amount <= 0)
                return "Ningún campo puede estar vacío.";

            var loans = _loanRepository.FindAllLoans();
            if (!loans.Any(l => l.Id == newLoanDTO.LoanId)) //Verificar que el ID del prestamo exista.
                return "El ID del préstamo no existe.";

            var requiredLoan = _loanRepository.FindLoanById(newLoanDTO.LoanId);
            if (newLoanDTO.Amount > requiredLoan.MaxAmount) //Verificar que el monto solicitado no supere al máximo del prestamo.
                return "El monto solicitado excede al máximo del préstamo.";

            var paymentList = requiredLoan.Payments.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(payment => int.Parse(payment.Trim()))
                        .ToList();
            var paymentToInt = int.Parse(newLoanDTO.Payments);

            if (!paymentList.Contains(paymentToInt)) //Verificar que la cantidad de cuotas solicitada coincida con las que brinda el prestamo.
                return "La cantidad de cuotas solicitada no es válida para este préstamo.";

            Client client = _clientRepository.FindClientByEmail(email);
            ClientDTO clientDTO = new ClientDTO(client);
            var allAccounts = _accountRepository.FindAllAccounts();
            var clientAccounts = clientDTO.Accounts;
            var loanAccount = clientAccounts.FirstOrDefault(a => a.Number == newLoanDTO.ToAccountNumber);

            if (allAccounts.Any(a => loanAccount.Number == newLoanDTO.ToAccountNumber)) //Verificar que la cuenta de destino exista.
            {
                if (!clientAccounts.Any(a => a.Number == newLoanDTO.ToAccountNumber)) //Verificar que la cuenta destino pertenezca al cliente.
                    return "La cuenta destino no pertenece a este cliente.";
            }
            else
                return "La cuenta destino no existe.";

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

            return "Prestamo aprobado correctamente.";
        }
    }
}
