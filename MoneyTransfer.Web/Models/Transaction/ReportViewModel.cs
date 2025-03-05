namespace MoneyTransfer.Web.Models.Transaction;

public class ReportViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<TransactionSummaryViewModel> Transactions { get; set; } = new List<TransactionSummaryViewModel>();
}