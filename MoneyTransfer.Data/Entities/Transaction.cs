using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MoneyTransfer.Data.Entities;

public class Transaction
{
    public Guid Id { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }

    [ForeignKey("Sender")]
    public Guid SenderId { get; set; }

    [ForeignKey("Receiver")]
    public Guid ReceiverId { get; set; }
    public decimal TransferAmount { get; set; } // in MYR
    public decimal ExchangeRate { get; set; }
    public decimal PayoutAmount { get; set; } // in NPR
    public DateTime TransactionDate { get; set; }
    public string Status { get; set; } // Initiated, Completed, Failed

    public virtual User User { get; set; }
    public virtual Sender Sender { get; set; }
    public virtual Receiver Receiver { get; set; }
}