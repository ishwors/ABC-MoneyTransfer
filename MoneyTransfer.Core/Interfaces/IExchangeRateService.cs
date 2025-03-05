using MoneyTransfer.Core.DTOs.ExchangeRate;

namespace MoneyTransfer.Core.Interfaces;

public interface IExchangeRateService
{
    Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency);
    Task<bool> UpdateExchangeRatesAsync();
    Task<IEnumerable<ExchangeRateDto>> GetAllCurrentRatesAsync();
}