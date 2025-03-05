using MoneyTransfer.Core.DTOs.Transaction;
using MoneyTransfer.Data.Entities;

namespace MoneyTransfer.Core.Interfaces;

public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(TransactionDto dto);
    Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? userId = null);
    Task<Transaction> GetTransactionByIdAsync(Guid id);
}