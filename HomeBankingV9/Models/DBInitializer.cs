using System.Linq.Expressions;

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
                    new Client { FirstName = "Matias", LastName = "Gambini", Email = "matiasgambini@gmail.com", Password = "1234" },
                    new Client { FirstName = "Maria", LastName = "Ferreyra", Email = "asd@gmail.com", Password = "2345" },
                    new Client { FirstName = "Juana", LastName = "Perez", Email = "dsa@gmail.com", Password = "3456" },
                    new Client { FirstName = "Martin", LastName = "Zuazo", Email = "sdasda@gmail.com", Password = "4567" },
                    new Client { FirstName = "Nicolás", LastName = "ASD", Email = "qwer@gmail.com", Password = "5678" },
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
                        new Account { Number = "VIN001", CreationDate = DateTime.Now, Balance = 100000, ClientId=matiClient.Id},
                        new Account { Number = "VIN002", CreationDate = DateTime.Now, Balance = 200000, ClientId=matiClient.Id}
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
        }
    }
}
