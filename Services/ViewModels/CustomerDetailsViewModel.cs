using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.ViewModels
{
    public class CustomerDetailsViewModel
    {
        public int CustomerId { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Givenname { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Streetaddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Zipcode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public DateOnly? Birthday { get; set; }
        public string? NationalId { get; set; }
        public string? Telephonecountrycode { get; set; }
        public string? Telephonenumber { get; set; }
        public string? Emailaddress { get; set; }

        public List<AccountDetailsViewModel> Accounts { get; set; } = new();

        public decimal TotalBalance => Accounts.Sum(a => a.Balance);

        public string Address => $"{Streetaddress}, {City}, {Zipcode}";
        public string Phone => $"{Telephonecountrycode} {Telephonenumber}".Trim();

        public string Name => $"{Givenname} {Surname}";
    }
}
