namespace MoneyTransfer.Web.Models.Transaction;

public class TransactionDetailsViewModel
{
    public Guid TransactionId { get; set; }
    public string SenderFullName { get; set; }
    public string ReceiverFullName { get; set; }
    public string BankName { get; set; }
    public string AccountNumber { get; set; }
    public decimal TransferAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal PayoutAmount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Status { get; set; }
}