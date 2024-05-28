﻿using System.Linq.Expressions;

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
                    Client eduClient = context.Clients.First(cl => cl.Email == "matiasgambini@gmail.com");
                if (eduClient != null)
                {
                    var eduAccounts = new Account[]
                    {
                        new Account { Number = "VIN001", CreationDate = DateTime.Now, Balance = 100000, ClientId=eduClient.Id},
                        new Account { Number = "VIN002", CreationDate = DateTime.Now, Balance = 200000, ClientId=eduClient.Id}
                    };
                    context.Accounts.AddRange(eduAccounts);
                    context.SaveChanges();
                }
            }
        }
    }
}
