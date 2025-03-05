namespace MoneyTransfer.Web.Models.ExchangeRate;

public class ExchangeRateViewModel
{
    public string FromCurrency { get; set; }
    public string ToCurrency { get; set; }
    public decimal Rate { get; set; }
    public DateTime LastUpdated { get; set; }
}
