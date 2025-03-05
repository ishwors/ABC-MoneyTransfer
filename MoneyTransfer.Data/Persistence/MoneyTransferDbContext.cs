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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}
