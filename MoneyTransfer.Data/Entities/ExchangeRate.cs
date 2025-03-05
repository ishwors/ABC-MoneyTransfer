namespace MoneyTransfer.Data.Entities;

public class ExchangeRate
{
    public Guid Id { get; set; }
    public string FromCurrency { get; set; }
    public string ToCurrency { get; set; }
    public decimal Rate { get; set; }
    public DateTime FetchDate { get; set; }
}