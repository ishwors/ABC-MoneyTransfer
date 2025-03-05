namespace MoneyTransfer.Web.Models.Transaction;

public class TransactionSummaryViewModel
{
    public Guid TransactionId { get; set; }
    public string ReceiverName { get; set; }
    public decimal TransferAmount { get; set; }
    public decimal PayoutAmount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Status { get; set; }
}
