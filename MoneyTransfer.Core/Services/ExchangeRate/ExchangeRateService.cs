using MoneyTransfer.Core.DTOs.ExchangeRate;
using MoneyTransfer.Core.Interfaces;
using MoneyTransfer.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MoneyTransfer.Core.Services.ExchangeRate;

public class ExchangeRateService : IExchangeRateService
{
    private readonly HttpClient _httpClient;
    private readonly MoneyTransferDbContext _dbContext;
    private readonly ILogger<ExchangeRateService> _logger;
    private readonly IConfiguration _configuration;

    public ExchangeRateService(
        HttpClient httpClient,
        MoneyTransferDbContext dbContext,
        ILogger<ExchangeRateService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
    {
        // For MYR to NPR conversion
        if (fromCurrency == "MYR" && toCurrency == "NPR")
        {
            // Get today's date
            var today = DateTime.Today;

            var directRate = await _dbContext.ExchangeRates
                .Where(r => r.FromCurrency == fromCurrency &&
                       r.ToCurrency == toCurrency &&
                       r.FetchDate.Date == today)
                .OrderByDescending(r => r.FetchDate)
                .FirstOrDefaultAsync();

            if (directRate != null)
            {
                return directRate.Rate;
            }

            var todaysRates = await _dbContext.ExchangeRates
                .Where(r => r.FetchDate.Date == today)
                .AnyAsync();

            if (!todaysRates)
            {
                await UpdateExchangeRatesAsync();
            }

            var usdToNpr = await _dbContext.ExchangeRates
                .Where(r => r.FromCurrency == "USD" &&
                       r.ToCurrency == "NPR" &&
                       r.FetchDate.Date == today)
                .OrderByDescending(r => r.FetchDate)
                .FirstOrDefaultAsync();

            if (usdToNpr == null)
            {
                _logger.LogWarning("No USD to NPR exchange rate found. Using default rate.");
                return 30.25m; 
            }

            decimal myrToUsd = 0.21m;

            decimal myrToNpr = myrToUsd * usdToNpr.Rate;

            var newRate = new Data.Entities.ExchangeRate
            {
                FromCurrency = "MYR",
                ToCurrency = "NPR",
                Rate = myrToNpr,
                FetchDate = DateTime.Now
            };

            await _dbContext.ExchangeRates.AddAsync(newRate);
            await _dbContext.SaveChangesAsync();

            return myrToNpr;
        }

        var rate = await _dbContext.ExchangeRates
            .Where(r => r.FromCurrency == fromCurrency &&
                   r.ToCurrency == toCurrency &&
                   r.FetchDate.Date == DateTime.Today)
            .OrderByDescending(r => r.FetchDate)
            .FirstOrDefaultAsync();

        if (rate != null)
        {
            return rate.Rate;
        }

        await UpdateExchangeRatesAsync();

        rate = await _dbContext.ExchangeRates
            .Where(r => r.FromCurrency == fromCurrency &&
                   r.ToCurrency == toCurrency)
            .OrderByDescending(r => r.FetchDate)
            .FirstOrDefaultAsync();

        if (rate != null)
        {
            return rate.Rate;
        }

        throw new Exception($"Exchange rate for {fromCurrency} to {toCurrency} not available.");
    }

    public async Task<bool> UpdateExchangeRatesAsync()
    {
        try
        {
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            var url = $"{_configuration["ExternalApi:ExchangeRateNRB"]}?page=1&per_page=100&from={today}&to={today}";

            _logger.LogInformation($"Fetching exchange rates from: {url}");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            // Deserialize using the corrected model structure
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rateData = JsonSerializer.Deserialize<NrbApiResponse>(content, jsonOptions);

            if (rateData?.Status?.Code != 200 ||
                rateData?.Data?.Payload == null ||
                !rateData.Data.Payload.Any())
            {
                _logger.LogWarning("No exchange rate data found from NRB API.");
                return false;
            }

            var latestRates = rateData.Data.Payload.FirstOrDefault();

            if (latestRates?.Rates == null || !latestRates.Rates.Any())
            {
                _logger.LogWarning("No rate details found in NRB API response.");
                return false;
            }

            var fetchDate = DateTime.Parse(latestRates.Date);

            var existingRates = await _dbContext.ExchangeRates
                .Where(r => r.FetchDate.Date == fetchDate.Date)
                .ToListAsync();

            if (existingRates.Any())
            {
                _dbContext.ExchangeRates.RemoveRange(existingRates);
                await _dbContext.SaveChangesAsync();
            }

            foreach (var rate in latestRates.Rates)
            {
                if (string.IsNullOrEmpty(rate.Buy) || string.IsNullOrEmpty(rate.Sell))
                {
                    continue;
                }

                if (decimal.TryParse(rate.Sell, out decimal sellRate))
                {
                    var adjustedRate = sellRate / rate.Currency.Unit;

                    var foreignToNpr = new Data.Entities.ExchangeRate
                    {
                        FromCurrency = rate.Currency.Iso3,
                        ToCurrency = "NPR",
                        Rate = adjustedRate,
                        FetchDate = fetchDate
                    };

                    await _dbContext.ExchangeRates.AddAsync(foreignToNpr);

                    var nprToForeign = new Data.Entities.ExchangeRate
                    {
                        FromCurrency = "NPR",
                        ToCurrency = rate.Currency.Iso3,
                        Rate = 1 / adjustedRate,
                        FetchDate = fetchDate
                    };

                    await _dbContext.ExchangeRates.AddAsync(nprToForeign);
                }
            }

            var usdRate = latestRates.Rates.FirstOrDefault(r => r.Currency.Iso3 == "USD");
            if (usdRate != null && decimal.TryParse(usdRate.Sell, out decimal usdNprRate))
            {
                decimal myrToUsd = 0.21m; // 1 MYR = 0.21 USD (as of March 2025)

                var myrToNpr = new Data.Entities.ExchangeRate
                {
                    FromCurrency = "MYR",
                    ToCurrency = "NPR",
                    Rate = myrToUsd * usdNprRate,
                    FetchDate = fetchDate
                };

                await _dbContext.ExchangeRates.AddAsync(myrToNpr);

                var nprToMyr = new Data.Entities.ExchangeRate
                {
                    FromCurrency = "NPR",
                    ToCurrency = "MYR",
                    Rate = 1 / (myrToUsd * usdNprRate),
                    FetchDate = fetchDate
                };

                await _dbContext.ExchangeRates.AddAsync(nprToMyr);
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Exchange rates updated successfully.");
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error connecting to NRB API.");
            return false;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing NRB API response.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating exchange rates.");
            return false;
        }
    }

    public async Task<IEnumerable<ExchangeRateDto>> GetAllCurrentRatesAsync()
    {
        var today = DateTime.Today;

        var todaysRates = await _dbContext.ExchangeRates
            .Where(r => r.FetchDate.Date == today && r.FromCurrency != "NPR")
            .AnyAsync();

        if (!todaysRates)
        {
            await UpdateExchangeRatesAsync();
        }

        var rates = await _dbContext.ExchangeRates
            .Where(r => r.ToCurrency == "NPR" && r.FromCurrency != "NPR")
            .OrderByDescending(r => r.FetchDate)
            .ThenBy(r => r.FromCurrency)
            .ToListAsync();

        var result = new List<ExchangeRateDto>();

        var latestRates = rates
            .GroupBy(r => r.FromCurrency)
            .Select(g => g.OrderByDescending(r => r.FetchDate).First());

        foreach (var rate in latestRates)
        {
            int unit = rate.FromCurrency == "INR" ? 100 : 1;

            result.Add(new ExchangeRateDto
            {
                CurrencyCode = rate.FromCurrency,
                Unit = unit,
                SellRate = rate.Rate * unit,
                BuyRate = rate.Rate * 0.995m * unit, // Approximate buy rate
                FetchDate = rate.FetchDate
            });
        }

        return result;
    }
}