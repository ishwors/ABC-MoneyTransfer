using MoneyTransfer.Core.DTOs;
using MoneyTransfer.Core.Interfaces;
using MoneyTransfer.Data.Entities;
using MoneyTransfer.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using MoneyTransfer.Core.Services.Helper;

namespace MoneyTransfer.Core.Services.ExchangeRate;

public class AuthService : IAuthService
{
    private readonly MoneyTransferDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private static EncryptionDecryption _encryptionDecryption;

    public AuthService(
        MoneyTransferDbContext dbContext,
        IConfiguration configuration,
        ILogger<AuthService> logger,
        EncryptionDecryption encryptionDecryption)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
        _encryptionDecryption = encryptionDecryption;
    }

    public async Task<User> RegisterUserAsync(RegisterDto registerDto)
    {
        var userch = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
        // Check if user already exists
        if (userch != null)
        {
            throw new InvalidOperationException("Email already in use");
        }

        // Hash password
        var hashingKey = _configuration.GetSection("Hash:PasswordHashSecret").Value;
        var passwordHash = await _encryptionDecryption.EncryptString(hashingKey, registerDto.Password);

        // Create new user
        var user = new User
        {
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            FirstName = registerDto.FirstName,
            MiddleName = registerDto.MiddleName,
            LastName = registerDto.LastName,
            Address = registerDto.Address,
            Country = registerDto.Country,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<(bool success, string token)> LoginAsync(LoginDto loginDto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null)
        {
            return (false, null);
        }

        // Verify password
        var hashingKey = _configuration.GetSection("Hash:PasswordHashSecret").Value;
        var passwordHash = await _encryptionDecryption.EncryptString(hashingKey, loginDto.Password);
        if (user.PasswordHash != passwordHash)
        {
            return (false, null);
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);

        return (true, token);
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}