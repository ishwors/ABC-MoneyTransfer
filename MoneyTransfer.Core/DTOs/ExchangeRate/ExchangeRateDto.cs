namespace MoneyTransfer.Core.DTOs.ExchangeRate;
public class ExchangeRateDto
{
    public string CurrencyCode { get; set; }
    public int Unit { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public DateTime FetchDate { get; set; }
}