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
    public class EmployeeRegistrationService : IEmployeeRegistrationService
    {
        private readonly MaverickBankContext _context;
        private readonly ILogger<EmployeeRegistrationService> _logger;

        public EmployeeRegistrationService(MaverickBankContext context, ILogger<EmployeeRegistrationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RegisterResponseDTO> RegisterEmployeeAsync(RegisterEmployeeDTO dto)
        {
            _logger.LogInformation("Starting employee registration for username: {Username}", dto.Username);

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                _logger.LogWarning("Username already exists: {Username}", dto.Username);
                throw new Exception("Username already exists");
            }

            var branch = await _context.Branches.FindAsync(dto.BranchId);
            if (branch == null)
            {
                _logger.LogWarning("Branch not found with ID: {BranchId}", dto.BranchId);
                throw new Exception("Branch not found");
            }

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                Role = "Employee"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User created with ID: {UserId}", user.UserId);

            var employee = new Employee
            {
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                BranchId = dto.BranchId,
                CreatedAt = DateTime.Now,
                UserId = user.UserId
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Employee profile created for User ID: {UserId}", user.UserId);

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
