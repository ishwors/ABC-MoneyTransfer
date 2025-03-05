using MoneyTransfer.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MoneyTransfer.Data.Persistence;

public class MoneyTransferDbContext : DbContext
{
    public MoneyTransferDbContext(DbContextOptions<MoneyTransferDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Sender> Senders { get; set; }
    public DbSet<Receiver> Receivers { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<ExchangeRate> ExchangeRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure decimal precision
        modelBuilder.Entity<ExchangeRate>()
        .Property(e => e.Rate)
        .HasPrecision(18, 6); // Adjust as needed

        modelBuilder.Entity<Transaction>()
            .Property(t => t.ExchangeRate)
            .HasPrecision(18, 6);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.PayoutAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.TransferAmount)
            .HasPrecision(18, 2);


        // Configure relationships
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.User)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Sender)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Receiver)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
