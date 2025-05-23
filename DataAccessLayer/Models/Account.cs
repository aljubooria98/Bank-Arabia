using DataAccessLayer.Models;

public class Account
{
    public int AccountId { get; set; }

    public string Frequency { get; set; } = null!;
    public DateOnly Created { get; set; }
    public decimal Balance { get; set; }
    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Disposition> Dispositions { get; set; } = new List<Disposition>();
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public virtual ICollection<PermenentOrder> PermenentOrders { get; set; } = new List<PermenentOrder>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
