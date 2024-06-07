using System.Drawing;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeBankingV9.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client { FirstName = "Matías", LastName = "Gambini", Email = "matiasgambini@gmail.com", Password = "1234" },
                    new Client { FirstName = "Facu", LastName = "Sommo", Email = "facusommo@gmail.com", Password = "4321" },
                    new Client { FirstName = "Maria", LastName = "Ferreyra", Email = "asd@gmail.com", Password = "2345" },
                    new Client { FirstName = "Juana", LastName = "Perez", Email = "dsa@gmail.com", Password = "3456" },
                };
                context.Clients.AddRange(clients);
                context.SaveChanges();
            }

            if (!context.Accounts.Any())
            {
                Client matiClient = context.Clients.FirstOrDefault(cl => cl.Email == "matiasgambini@gmail.com");
                if (matiClient != null)
                {
                    var matiAccounts = new Account[]
                    {
                        new Account { Number = "VIN-00000001", CreationDate = DateTime.Now, Balance = 100000, ClientId=matiClient.Id},
                        new Account { Number = "VIN-00000002", CreationDate = DateTime.Now, Balance = 200000, ClientId=matiClient.Id}
                    };
                    context.Accounts.AddRange(matiAccounts);
                    context.SaveChanges();
                }
            }

            if (!context.Transactions.Any())
            {
                Account matiAccount = context.Accounts.FirstOrDefault(acc => acc.Number == "VIN001");
                if (matiAccount != null)
                {
                    var matiTransactions = new Transaction[]
                    {
                        new Transaction {Type = TransactionType.DEBIT, Amount = -1000, Description = "Compra en supermercado Buenos días.",
                            Date= DateTime.Now.AddDays(-3), AccountId=matiAccount.Id},
                        new Transaction {Type = TransactionType.CREDIT, Amount = 5000, Description = "Transferencia recibida cuenta propia.",
                            Date= DateTime.Now.AddHours(+4), AccountId=matiAccount.Id},
                        new Transaction {Type = TransactionType.CREDIT, Amount = 1000, Description = "Pago de haberes mayo.",
                            Date= DateTime.Now.AddHours(+1), AccountId=matiAccount.Id}
                    };
                    context.Transactions.AddRange(matiTransactions);
                    context.SaveChanges();
                }
            }

            if (!context.Loans.Any())
            {
                    var matiLoans = new Loan[]
                    {
                        new Loan { Name = "Hipotecario", MaxAmount = 500000, Payments = "12,24,36,48,60" },
                        new Loan { Name = "Personal", MaxAmount = 100000, Payments = "6,12,24" },
                        new Loan { Name = "Prendario", MaxAmount = 300000, Payments = "6,12,24,36" },
                    };
                    context.Loans.AddRange(matiLoans);
                    context.SaveChanges();
            }
            
            if (!context.ClientLoans.Any())
            {
                Account matiAccount = context.Accounts.FirstOrDefault(acc => acc.Number == "VIN-00000001");
                if (matiAccount != null)
                {
                    var loanHipotecario = context.Loans.FirstOrDefault(hi => hi.Name == "Hipotecario");
                    if (loanHipotecario != null)
                    {
                        var loanHi = new ClientLoan
                        { Amount = 380000, Payments = "36", ClientId = matiAccount.Id, LoanId = loanHipotecario.Id };
                        context.ClientLoans.Add(loanHi);
                    }
                    var loanPersonal = context.Loans.FirstOrDefault(pe => pe.Name == "Personal");
                    if (loanPersonal != null)
                    {
                        var loanPe = new ClientLoan
                        { Amount = 75000, Payments = "6", ClientId = matiAccount.Id, LoanId = loanPersonal.Id };
                        context.ClientLoans.Add(loanPe);
                    }
                    var loanPrendario = context.Loans.FirstOrDefault(pr => pr.Name == "Prendario");
                    if (loanPrendario != null)
                    {
                        var loanPr = new ClientLoan
                        { Amount = 230000, Payments = "24", ClientId = matiAccount.Id, LoanId = loanPrendario.Id };
                        context.ClientLoans.Add(loanPr);
                    }
                    context.SaveChanges();
                }  
            }
            if (!context.Cards.Any())
            {
                Client matiClient = context.Clients.FirstOrDefault(acc => acc.LastName == "Gambini");
                if (matiClient != null)
                {
                    var matiCards = new Card[]
                    {
                        new Card { CardHolder = matiClient.FirstName+" "+matiClient.LastName,
                            Type = CardType.DEBIT, Color = CardColor.GOLD, Number = "1234-5678-9012-3456", Cvv = 123,
                            FromDate = DateTime.Now.AddDays(-5), ThruDate = DateTime.Now.AddYears(+5), ClientId = matiClient.Id },
                        new Card { CardHolder = matiClient.FirstName+" "+matiClient.LastName,
                            Type = CardType.CREDIT, Color = CardColor.TITANIUM, Number = "6543-2109-8765-4321", Cvv = 321,
                            FromDate = DateTime.Now.AddDays(-4), ThruDate = DateTime.Now.AddYears(+4), ClientId = matiClient.Id }
                    };
                    context.Cards.AddRange(matiCards);
                    context.SaveChanges();
                }
            }
        }
    }
}
