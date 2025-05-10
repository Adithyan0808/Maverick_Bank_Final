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
    public class AuthenticationService : IAuthenticationService
    {
        private readonly MaverickBankContext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(MaverickBankContext context, ITokenService tokenService, ILogger<AuthenticationService> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<LoginResponse> Login(UserLoginRequest loginRequest)
        {
            _logger.LogInformation("Login attempt for username: {Username}", loginRequest.Username);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for username {Username}", loginRequest.Username);
                throw new Exception("User not found");
            }

            var inputPasswordHash = HashPassword(loginRequest.Password);
            if (user.PasswordHash != inputPasswordHash)
            {
                _logger.LogWarning("Login failed: Invalid password for username {Username}", loginRequest.Username);
                throw new UnauthorizedAccessException("Invalid password");
            }

            var token = await _tokenService.GenerateToken(user.UserId, user.Username, user.Role);
            int responseId = user.UserId;

            try
            {
                if (user.Role == "Customer")
                {
                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.UserId);
                    if (customer != null) responseId = customer.CustomerId;
                }
                else if (user.Role == "Employee")
                {
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == user.UserId);
                    if (employee != null) responseId = employee.EmployeeId;
                }
                else if (user.Role == "Admin")
                {
                    var admin = await _context.Admins.FirstOrDefaultAsync(a => a.UserId == user.UserId);
                    if (admin != null) responseId = admin.AdminId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while resolving role-specific ID for user {Username}", user.Username);
                throw;
            }

            _logger.LogInformation("User {Username} logged in successfully with role {Role}", user.Username, user.Role);

            return new LoginResponse
            {
                Id = responseId,
                Username = user.Username,
                Role = user.Role,
                Token = token
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
