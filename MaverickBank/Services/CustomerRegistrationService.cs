using MaverickBank.Contexts;
using MaverickBank.Interfaces;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace MaverickBank.Services
{
    public class CustomerRegistrationService : ICustomerRegistrationService
    {
        private readonly MaverickBankContext _context;
        private readonly ILogger<CustomerRegistrationService> _logger;

        public CustomerRegistrationService(MaverickBankContext context, ILogger<CustomerRegistrationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RegisterResponseDTO> RegisterCustomerAsync(RegisterCustomerDTO dto)
        {
            _logger.LogInformation("Starting customer registration for username: {Username}", dto.Username);

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                _logger.LogWarning("Registration failed: Username '{Username}' already exists", dto.Username);
                throw new Exception("Username already exists");
            }

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                Role = "Customer"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User created with ID: {UserId}", user.UserId);

            var customer = new Customer
            {
                FullName = dto.FullName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                AadharNumber = dto.AadharNumber,
                PANNumber = dto.PANNumber,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Address = dto.Address,
                CreatedAt = DateTime.Now,
                UserId = user.UserId
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Customer profile created with ID: {CustomerId}", customer.CustomerId);

            var account = new Account
            {
                AccountNumber = dto.AccountNumber,
                Balance = 0,
                AccountTypeId = dto.AccountTypeId,
                CustomerId = customer.CustomerId
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Account created with number: {AccountNumber}", account.AccountNumber);

            _logger.LogInformation("Customer registration successful for username: {Username}", user.Username);

            return new RegisterResponseDTO
            {
                Username = user.Username,
                Role = user.Role,
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}








