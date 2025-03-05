using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.Web.Models.Auth;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Display(Name = "Middle Name")]
    public string? MiddleName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required]
    [Display(Name = "Address")]
    public string Address { get; set; }

    [Required]
    [Display(Name = "Country")]
    public string Country { get; set; }
}
