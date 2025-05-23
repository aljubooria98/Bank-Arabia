namespace Services.ViewModels
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Operation { get; set; } = string.Empty;
    }
}
