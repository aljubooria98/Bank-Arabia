namespace Services.ViewModels
{
    public class TransactionInputViewModel
    {
        public int FromAccountId { get; set; }
        public int? ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Type) &&
            Amount > 0 &&
            (Type != "Transfer" || ToAccountId.HasValue);
    }
}
