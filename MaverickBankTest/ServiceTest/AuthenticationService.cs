using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MaverickBank.Contexts;
using MaverickBank.Interfaces;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using MaverickBank.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MaverickBankTest.ServiceTest
{
    public class AuthenticationServiceTests
    {
        private MaverickBankContext _context;
        private Mock<ITokenService> _mockTokenService;
        private Mock<ILogger<AuthenticationService>> _mockLogger;
        private AuthenticationService _authService;

        private const string ValidPassword = "Test@123";
        private string ValidHashedPassword;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MaverickBankContext(options);

            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(ValidPassword));
            ValidHashedPassword = Convert.ToBase64String(hash);

            _mockTokenService = new Mock<ITokenService>();
            _mockTokenService.Setup(t => t.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                             .ReturnsAsync("mocked-token");

            _mockLogger = new Mock<ILogger<AuthenticationService>>();

            _authService = new AuthenticationService(_context, _mockTokenService.Object, _mockLogger.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Login_WithValidCustomerCredentials_ReturnsLoginResponseWithCustomerId()
        {
            var user = new User { Username = "customer1", PasswordHash = ValidHashedPassword, Role = "Customer" };

            var customer = new Customer
            {
                User = user,
                FullName = "Test Customer",
                PhoneNumber = "1234567890",
                Email = "test@example.com",
                AadharNumber = "123412341234",
                PANNumber = "ABCDE1234F",
                Address = "123 Test Street",
                Gender = "Male"
            };

            _context.Users.Add(user);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var loginRequest = new UserLoginRequest { Username = "customer1", Password = ValidPassword };

            var result = await _authService.Login(loginRequest);

            Assert.NotNull(result);
            Assert.AreEqual(user.Username, result.Username);
            Assert.AreEqual("Customer", result.Role);
            Assert.AreEqual("mocked-token", result.Token);
            Assert.AreEqual(customer.CustomerId, result.Id);
        }

        [Test]
        public void Login_WithInvalidUsername_ThrowsException()
        {
            var request = new UserLoginRequest { Username = "notfound", Password = "any" };

            var ex = Assert.ThrowsAsync<Exception>(async () => await _authService.Login(request));
            Assert.That(ex.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task Login_WithInvalidPassword_ThrowsUnauthorizedAccessException()
        {
            var user = new User
            {
                Username = "testuser",
                PasswordHash = "invalidpasswordhash",
                Role = "Employee"
            };

            var employee = new Employee
            {
                User = user,
                FullName = "Test Employee",
                PhoneNumber = "1234567890",
                Email = "test@employee.com",
                BranchId = 1,
                UserId = user.UserId
            };

            _context.Users.Add(user);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var exception = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _authService.Login(new UserLoginRequest
                {
                    Username = "testuser",
                    Password = "incorrectpassword"
                })
            );

            Assert.AreEqual("Invalid password", exception.Message);
        }

        [Test]
        public async Task Login_WithEmployeeRole_ReturnsLoginResponseWithEmployeeId()
        {
            var password = "validpassword";
            var passwordHash = HashPassword1(password);

            var user = new User
            {
                Username = "testuser",
                PasswordHash = passwordHash,
                Role = "Employee"
            };

            var employee = new Employee
            {
                User = user,
                FullName = "Test Employee",
                PhoneNumber = "1234567890",
                Email = "test@employee.com",
                BranchId = 1,
                UserId = user.UserId
            };

            _context.Users.Add(user);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var loginResponse = await _authService.Login(new UserLoginRequest
            {
                Username = "testuser",
                Password = password
            });

            Assert.AreEqual(employee.EmployeeId, loginResponse.Id);
            Assert.AreEqual(user.Username, loginResponse.Username);
            Assert.AreEqual(user.Role, loginResponse.Role);
            Assert.AreEqual("mocked-token", loginResponse.Token);
        }

        private string HashPassword1(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
