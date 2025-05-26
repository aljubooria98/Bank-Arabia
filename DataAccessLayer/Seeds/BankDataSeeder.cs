using Bogus;
using DataAccessLayer.Data;
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
                return; // Hoppa över om det redan finns kunder

            var countries = new[] { "Sweden", "Norway", "Denmark", "Finland", "Germany", "France", "Italy", "Spain", "Netherlands", "Belgium" };
            var transactionTypes = new[] { "Deposit", "Withdraw" };

            var customerFaker = new Faker<Customer>("en")
                .RuleFor(c => c.Givenname, f => f.Name.FirstName())
                .RuleFor(c => c.Surname, f => f.Name.LastName())
                .RuleFor(c => c.Emailaddress, (f, c) => f.Internet.Email(c.Givenname, c.Surname))
                .RuleFor(c => c.Telephonenumber, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.NationalId, f => f.Random.Replace("######-####"))
                .RuleFor(c => c.Country, f => f.PickRandom(countries))
                .RuleFor(c => c.Streetaddress, f => f.Address.StreetAddress())
                .RuleFor(c => c.City, f => f.Address.City())
                .RuleFor(c => c.Zipcode, f => f.Address.ZipCode())
                .RuleFor(c => c.CountryCode, f => f.Address.CountryCode())
                .RuleFor(c => c.Birthday, f => DateOnly.FromDateTime(f.Date.Past(60, DateTime.Now.AddYears(-18))))
                .RuleFor(c => c.Gender, f => f.PickRandom(new[] { "M", "F" }))
                .RuleFor(c => c.Telephonecountrycode, f => "+" + f.Random.Number(1, 99).ToString())
                .RuleFor(c => c.Dispositions, _ => new List<Disposition>());

            var customers = new List<Customer>();

            for (int i = 0; i < 200; i++)
            {
                var customer = customerFaker.Generate();
                var dispositions = new List<Disposition>();

                for (int j = 0; j < 2; j++)
                {
                    var account = new Account
                    {
                        Created = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6)),
                        Frequency = "Monthly",
                        Balance = 0,
                        Transactions = new List<Transaction>()
                    };

                    var transactionFaker = new Faker();

                    for (int k = 0; k < 50; k++)
                    {
                        var type = transactionFaker.PickRandom(transactionTypes);
                        var amount = transactionFaker.Finance.Amount(10, 2000);

                        account.Balance += (type == "Deposit") ? amount : -amount;

                        account.Transactions.Add(new Transaction
                        {
                            Amount = (type == "Deposit") ? amount : -amount,
                            Date = DateOnly.FromDateTime(transactionFaker.Date.Past(1)),
                            Type = type,
                            Operation = "Standard",
                            Balance = account.Balance
                        });
                    }

                    dispositions.Add(new Disposition
                    {
                        Account = account,
                        Customer = customer,
                        Type = "Owner"
                    });
                }

                customer.Dispositions = dispositions;
                customers.Add(customer);
            }

            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();
        }
    }
}
