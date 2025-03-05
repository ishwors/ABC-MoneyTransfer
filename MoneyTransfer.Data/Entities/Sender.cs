using System.Transactions;

namespace MoneyTransfer.Data.Entities;

public class Sender
{
    public Guid SenderId { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string Country { get; set; }
}
