using MoneyTransfer.Core.DTOs;
using MoneyTransfer.Data.Entities;

namespace MoneyTransfer.Core.Interfaces;

public interface IAuthService
{
    Task<User> RegisterUserAsync(RegisterDto registerDto);
    Task<(bool success, string token)> LoginAsync(LoginDto loginDto);
}