namespace MoneyTransfer.Data.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string Country { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}
