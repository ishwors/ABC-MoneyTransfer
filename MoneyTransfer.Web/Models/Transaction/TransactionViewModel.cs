using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.Web.Models.Transaction
{
    public class TransactionViewModel
    {
        [Required(ErrorMessage = "Sender First Name is required")]
        public string SenderFirstName { get; set; }

        public string? SenderMiddleName { get; set; }

        [Required(ErrorMessage = "Sender Last Name is required")]
        public string SenderLastName { get; set; }

        [Required(ErrorMessage = "Sender Address is required")]
        public string SenderAddress { get; set; }

        [Required(ErrorMessage = "Sender Country is required")]
        public string SenderCountry { get; set; }

        [Required(ErrorMessage = "Receiver First Name is required")]
        public string ReceiverFirstName { get; set; }

        public string? ReceiverMiddleName { get; set; }

        [Required(ErrorMessage = "Receiver Last Name is required")]
        public string ReceiverLastName { get; set; }

        [Required(ErrorMessage = "Receiver Address is required")]
        public string ReceiverAddress { get; set; }

        [Required(ErrorMessage = "Receiver Country is required")]
        public string ReceiverCountry { get; set; }

        [Required(ErrorMessage = "Receiver Bank Name is required")]
        public string ReceiverBankName { get; set; }

        [Required(ErrorMessage = "Receiver Account Number is required")]
        public string ReceiverAccountNumber { get; set; }

        [Required(ErrorMessage = "Transfer Amount is required")]
        public decimal TransferAmount { get; set; }
    }
}