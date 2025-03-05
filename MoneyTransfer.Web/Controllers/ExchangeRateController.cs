using MoneyTransfer.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.Web.Models.ExchangeRate;
using Microsoft.AspNetCore.Authorization;

namespace MoneyTransfer.Web.Controllers
{
    public class ExchangeRateController : Controller
    {
        private readonly IExchangeRateService _exchangeRateService;

        public ExchangeRateController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var rate = await _exchangeRateService.GetExchangeRateAsync("MYR", "NPR");

            var model = new ExchangeRateViewModel
            {
                FromCurrency = "MYR",
                ToCurrency = "NPR",
                Rate = rate,
                LastUpdated = DateTime.Now
            };

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetExchangeRate()
        {
            var rate = await _exchangeRateService.GetExchangeRateAsync("MYR", "NPR");
            return Json(new { rate });
        }
    }
}
