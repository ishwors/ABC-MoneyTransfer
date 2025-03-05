﻿using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.Web.Models.Auth;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}