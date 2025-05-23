using Bogus;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Seeds
{
    public static class BankDataSeeder
    {
        public static async Task SeedBankDataAsync(BankAppDataContext context)
        {
            if (await context.Customers.AnyAsync())
                return;

            var countries = new[] { "Sweden", "Norway", "Denmark", "Finland", "Germany", "France", "Italy", "Spain", "Netherlands", "Belgium" };
            var transactionTypes = new[] { "Deposit", "Withdraw" };

            var customerFaker = new Faker<Customer>("en")
                .RuleFor(c => c.Givenname, f => f.Name.FirstName())
                .RuleFor(c => c.Surname, f => f.Name.LastName())
                .RuleFor(c => c.Emailaddress, f => f.Internet.Email())
                .RuleFor(c => c.Telephonenumber, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.NationalId, f => f.Random.Replace("######-####"))
                .RuleFor(c => c.Streetaddress, f => f.Address.StreetAddress())
                .RuleFor(c => c.City, f => f.Address.City())
                .RuleFor(c => c.Zipcode, f => f.Address.ZipCode())
                .RuleFor(c => c.CountryCode, f => f.Address.CountryCode())
                .RuleFor(c => c.Birthday, f => DateOnly.FromDateTime(f.Date.Past(60, DateTime.Now.AddYears(-18))))
                .RuleFor(c => c.Gender, f => f.PickRandom(new[] { "M", "F" }))
                .RuleFor(c => c.Telephonecountrycode, f => "+" + f.Random.Number(1, 99).ToString())
                .RuleFor(c => c.Dispositions, f => new List<Disposition>());

            var customers = new List<Customer>();

            foreach (var country in countries)
            {
                for (int i = 0; i < 20; i++) // 20 kunder per land × 10 länder = 200 kunder
                {
                    var customer = customerFaker.Generate();
                    customer.Country = country;

                    var accounts = new List<Account>();
                    var dispositions = new List<Disposition>();

                    for (int j = 0; j < 2; j++)
                    {
                        var account = new Account
                        {
                            Balance = 0,
                            Frequency = "Monthly",
                            Created = DateOnly.FromDateTime(DateTime.Now),
                            Transactions = new List<Transaction>()
                        };

                        var transactionFaker = new Faker();
                        decimal runningBalance = 0m;

                        for (int k = 0; k < 50; k++)
                        {
                            var type = transactionFaker.PickRandom(transactionTypes) ?? "Deposit";
                            var amount = transactionFaker.Finance.Amount(10, 2000);

                            if (type == "Deposit")
                                runningBalance += amount;
                            else
                                runningBalance -= amount;

                            account.Balance = runningBalance;

                            account.Transactions.Add(new Transaction
                            {
                                Amount = (type == "Deposit") ? amount : -amount,
                                Date = DateOnly.FromDateTime(transactionFaker.Date.Past(1)),
                                Type = type,
                                Operation = type,
                                Balance = runningBalance
                            });
                        }

                        accounts.Add(account);

                        dispositions.Add(new Disposition
                        {
                            Customer = customer,
                            Account = account,
                            Type = "Owner"
                        });
                    }

                    customer.Accounts = accounts;
                    customer.Dispositions = dispositions;
                    customers.Add(customer);
                }
            }

            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();
        }
    }
}
