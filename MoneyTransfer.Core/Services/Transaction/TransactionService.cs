using MoneyTransfer.Core.DTOs.Transaction;
using MoneyTransfer.Core.Interfaces;
using MoneyTransfer.Data.Entities;
using MoneyTransfer.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MoneyTransfer.Core.Services.Transaction;


public class TransactionService : ITransactionService
{
    private readonly MoneyTransferDbContext _dbContext;
    private readonly IExchangeRateService _exchangeRateService;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        MoneyTransferDbContext dbContext,
        IExchangeRateService exchangeRateService,
        ILogger<TransactionService> logger)
    {
        _dbContext = dbContext;
        _exchangeRateService = exchangeRateService;
        _logger = logger;
    }

    public async Task<Data.Entities.Transaction> CreateTransactionAsync(TransactionDto dto)
    {
        try
        {
            // Get current exchange rate
            var exchangeRate = await _exchangeRateService.GetExchangeRateAsync("MYR", "NPR");

            // Calculate payout amount
            var payoutAmount = dto.TransferAmount * exchangeRate;

            // Create new sender if not exists
            var sender = await _dbContext.Senders.FirstOrDefaultAsync(s =>
                s.FirstName == dto.SenderFirstName &&
                s.LastName == dto.SenderLastName &&
                s.Address == dto.SenderAddress &&
                s.Country == dto.SenderCountry);

            if (sender == null)
            {
                sender = new Sender
                {
                    FirstName = dto.SenderFirstName,
                    MiddleName = dto.SenderMiddleName,
                    LastName = dto.SenderLastName,
                    Address = dto.SenderAddress,
                    Country = dto.SenderCountry
                };

                await _dbContext.Senders.AddAsync(sender);
                await _dbContext.SaveChangesAsync();
            }

            // Create new receiver if not exists
            var receiver = await _dbContext.Receivers.FirstOrDefaultAsync(r =>
                r.FirstName == dto.ReceiverFirstName &&
                r.LastName == dto.ReceiverLastName &&
                r.BankName == dto.ReceiverBankName &&
                r.AccountNumber == dto.ReceiverAccountNumber);

            if (receiver == null)
            {
                receiver = new Receiver
                {
                    FirstName = dto.ReceiverFirstName,
                    MiddleName = dto.ReceiverMiddleName,
                    LastName = dto.ReceiverLastName,
                    Address = dto.ReceiverAddress,
                    Country = dto.ReceiverCountry,
                    BankName = dto.ReceiverBankName,
                    AccountNumber = dto.ReceiverAccountNumber
                };

                await _dbContext.Receivers.AddAsync(receiver);
                await _dbContext.SaveChangesAsync();
            }

            // Create transaction
            var transaction = new Data.Entities.Transaction
            {
                UserId = dto.UserId,
                SenderId = sender.SenderId,
                ReceiverId = receiver.ReceiverId,
                TransferAmount = dto.TransferAmount,
                ExchangeRate = exchangeRate,
                PayoutAmount = payoutAmount,
                TransactionDate = DateTime.UtcNow,
                Status = "Initiated"
            };

            await _dbContext.Transactions.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();

            // In a real system, we'd have additional processing here
            // For example, calling an external service to process the transfer

            // Update status to completed (simplified for this example)
            transaction.Status = "Completed";
            await _dbContext.SaveChangesAsync();

            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            throw;
        }
    }

    public async Task<IEnumerable<Data.Entities.Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? userId = null)
    {
        var query = _dbContext.Transactions
            .Include(t => t.Sender)
            .Include(t => t.Receiver)
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate);

        if (userId.HasValue)
        {
            query = query.Where(t => t.UserId == userId.Value);
        }

        return await query.OrderByDescending(t => t.TransactionDate).ToListAsync();
    }

    public async Task<Data.Entities.Transaction> GetTransactionByIdAsync(Guid id)
    {
        return await _dbContext.Transactions
            .Include(t => t.Sender)
            .Include(t => t.Receiver)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}