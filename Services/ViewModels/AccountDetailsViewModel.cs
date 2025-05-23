using System;
using System.Collections.Generic;

namespace Services.ViewModels
{
    public class AccountDetailsViewModel
    {
        public int AccountId { get; set; }
        public decimal Balance { get; set; }

        public string AccountType { get; set; } = string.Empty; 
        public DateOnly? Created { get; set; } 
        public List<TransactionViewModel> Transactions { get; set; } = new();

        public int TransactionCount => Transactions?.Count ?? 0;
    }
}
