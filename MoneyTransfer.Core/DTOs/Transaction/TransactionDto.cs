namespace MoneyTransfer.Core.DTOs.Transaction;

public class TransactionDto
{
    public Guid UserId { get; set; }
    public string SenderFirstName { get; set; }
    public string? SenderMiddleName { get; set; }
    public string SenderLastName { get; set; }
    public string SenderAddress { get; set; }
    public string SenderCountry { get; set; }
    public string ReceiverFirstName { get; set; }
    public string? ReceiverMiddleName { get; set; }
    public string ReceiverLastName { get; set; }
    public string ReceiverAddress { get; set; }
    public string ReceiverCountry { get; set; }
    public string ReceiverBankName { get; set; }
    public string ReceiverAccountNumber { get; set; }
    public decimal TransferAmount { get; set; }
}
