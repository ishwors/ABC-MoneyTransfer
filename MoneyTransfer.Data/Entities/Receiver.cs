namespace MoneyTransfer.Data.Entities;

public class Receiver
{
    public Guid ReceiverId { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string Country { get; set; }
    public string BankName { get; set; }
    public string AccountNumber { get; set; }
}
