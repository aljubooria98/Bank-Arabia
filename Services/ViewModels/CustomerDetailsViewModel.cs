using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.ViewModels
{
    public class CustomerDetailsViewModel
    {
        public int CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public List<AccountViewModel> Accounts { get; set; } = new();

        public decimal TotalBalance => Accounts.Sum(a => a.Balance);
    }

    public class AccountViewModel
    {
        public int AccountId { get; set; }

        public decimal Balance { get; set; }

        public List<TransactionViewModel> Transactions { get; set; } = new();
    }
}