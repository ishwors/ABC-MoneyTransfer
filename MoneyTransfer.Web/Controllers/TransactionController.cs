using MoneyTransfer.Core.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.Core.DTOs.Transaction;
using MoneyTransfer.Web.Models.Transaction;
using MoneyTransfer.Web.Models.ExchangeRate;
using Microsoft.AspNetCore.Authorization;

namespace MoneyTransfer.Web.Controllers
{
    //[Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IExchangeRateService _exchangeRateService;

        public TransactionController(
            ITransactionService transactionService,
            IExchangeRateService exchangeRateService)
        {
            _transactionService = transactionService;
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var transactionDto = new TransactionDto
                {
                    UserId = Guid.Parse("EFBDFF31-14AA-4DFB-BECD-08DD5BEF4FA2"),
                    SenderFirstName = model.SenderFirstName,
                    SenderMiddleName = model.SenderMiddleName,
                    SenderLastName = model.SenderLastName,
                    SenderAddress = model.SenderAddress,
                    SenderCountry = model.SenderCountry,
                    ReceiverFirstName = model.ReceiverFirstName,
                    ReceiverMiddleName = model.ReceiverMiddleName,
                    ReceiverLastName = model.ReceiverLastName,
                    ReceiverAddress = model.ReceiverAddress,
                    ReceiverCountry = model.ReceiverCountry,
                    ReceiverBankName = model.ReceiverBankName,
                    ReceiverAccountNumber = model.ReceiverAccountNumber,
                    TransferAmount = model.TransferAmount
                };

                var transaction = await _transactionService.CreateTransactionAsync(transactionDto);

                return RedirectToAction("Details", new { id = transaction.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            var model = new TransactionDetailsViewModel
            {
                TransactionId = transaction.Id,
                SenderFullName = $"{transaction.Sender.FirstName} {transaction.Sender.LastName}",
                ReceiverFullName = $"{transaction.Receiver.FirstName} {transaction.Receiver.LastName}",
                BankName = transaction.Receiver.BankName,
                AccountNumber = transaction.Receiver.AccountNumber,
                TransferAmount = transaction.TransferAmount,
                ExchangeRate = transaction.ExchangeRate,
                PayoutAmount = transaction.PayoutAmount,
                TransactionDate = transaction.TransactionDate,
                Status = transaction.Status
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Report(ReportViewModel model)
        {
            if (model.StartDate == DateTime.MinValue)
            {
                model.StartDate = DateTime.Today.AddDays(-30);
            }

            if (model.EndDate == DateTime.MinValue)
            {
                model.EndDate = DateTime.Today;
            }

            var transactions = await _transactionService.GetTransactionsByDateRangeAsync(model.StartDate, model.EndDate, null);

            model.Transactions = transactions.Select(t => new TransactionSummaryViewModel
            {
                TransactionId = t.Id,
                ReceiverName = $"{t.Receiver.FirstName} {t.Receiver.LastName}",
                TransferAmount = t.TransferAmount,
                PayoutAmount = t.PayoutAmount,
                TransactionDate = t.TransactionDate,
                Status = t.Status
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ExchangeRate()
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
