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
    public class AdminRegistrationService : IAdminRegistrationService
    {
        private readonly MaverickBankContext _context;
        private readonly ILogger<AdminRegistrationService> _logger;

        public AdminRegistrationService(MaverickBankContext context, ILogger<AdminRegistrationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RegisterResponseDTO> RegisterAdminAsync(RegisterAdminDTO dto)
        {
            _logger.LogInformation("Starting admin registration for username: {Username}", dto.Username);

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                _logger.LogWarning("Registration failed: Username {Username} already exists", dto.Username);
                throw new Exception("Username already exists");
            }

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                Role = "Admin"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User entity created with ID: {UserId}", user.UserId);

            var admin = new Admin
            {
                FullName = dto.FullName,
                Email = dto.Email,
                CreatedAt = DateTime.Now,
                UserId = user.UserId
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Admin profile created for user ID: {UserId}", user.UserId);

            return new RegisterResponseDTO
            {
                Username = user.Username,
                Role = user.Role
            };
        }

        private string HashPassword(string password)
        {
            _logger.LogDebug("Hashing password using SHA256");
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
