using Moq;
using NUnit.Framework;
using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using MaverickBank.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MaverickBankTest.ServiceTest
{
    [TestFixture]
    public class EmployeeRegistrationServiceTests
    {
        private MaverickBankContext _context;
        private EmployeeRegistrationService _service;
        private Mock<ILogger<EmployeeRegistrationService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            _context = new MaverickBankContext(options);

            _loggerMock = new Mock<ILogger<EmployeeRegistrationService>>();

            _service = new EmployeeRegistrationService(_context, _loggerMock.Object);

            SeedDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void SeedDatabase()
        {
            _context.Branches.Add(new Branch { BranchId = 1, BranchName = "Main Branch", Location = "Downtown" });
            _context.SaveChanges();
        }

        [Test]
        public async Task RegisterEmployeeAsync_ShouldRegisterEmployeeSuccessfully()
        {
            var registerDto = new RegisterEmployeeDTO
            {
                Username = "john_doe_employee",
                Password = "Password123",
                FullName = "John Doe",
                PhoneNumber = "9876543210",
                Email = "john.doe@example.com",
                BranchId = 1
            };

            var result = await _service.RegisterEmployeeAsync(registerDto);

            Assert.NotNull(result);
            Assert.AreEqual("john_doe_employee", result.Username);
            Assert.AreEqual("Employee", result.Role);

            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == "john_doe_employee");
            Assert.NotNull(userInDb);
            Assert.AreEqual("john_doe_employee", userInDb.Username);
            Assert.AreEqual("Employee", userInDb.Role);

            var employeeInDb = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userInDb.UserId);
            Assert.NotNull(employeeInDb);
            Assert.AreEqual("John Doe", employeeInDb.FullName);
        }

        [Test]
        public async Task RegisterEmployeeAsync_ShouldThrowException_WhenUsernameAlreadyExists()
        {
            var existingUser = new User
            {
                Username = "john_doe_employee",
                PasswordHash = "hashedPassword",
                Role = "Employee"
            };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var registerDto = new RegisterEmployeeDTO
            {
                Username = "john_doe_employee",
                Password = "Password123",
                FullName = "Jane Doe",
                PhoneNumber = "9876543210",
                Email = "jane.doe@example.com",
                BranchId = 1
            };

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.RegisterEmployeeAsync(registerDto));
            Assert.AreEqual("Username already exists", ex.Message);
        }

        [Test]
        public async Task RegisterEmployeeAsync_ShouldThrowException_WhenBranchNotFound()
        {
            var registerDto = new RegisterEmployeeDTO
            {
                Username = "jane_smith_employee",
                Password = "Password123",
                FullName = "Jane Smith",
                PhoneNumber = "9876543211",
                Email = "jane.smith@example.com",
                BranchId = 999 // Non-existent branch
            };

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.RegisterEmployeeAsync(registerDto));
            Assert.AreEqual("Branch not found", ex.Message);
        }

        [Test]
        public async Task RegisterEmployeeAsync_ShouldCreateEmployeeWithBranch()
        {
            var registerDto = new RegisterEmployeeDTO
            {
                Username = "alice_smith_employee",
                Password = "SecurePassword123",
                FullName = "Alice Smith",
                PhoneNumber = "9876543211",
                Email = "alice.smith@example.com",
                BranchId = 1
            };

            var result = await _service.RegisterEmployeeAsync(registerDto);

            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == "alice_smith_employee");
            var employeeInDb = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userInDb.UserId);

            Assert.NotNull(userInDb);
            Assert.NotNull(employeeInDb);
            Assert.AreEqual("Alice Smith", employeeInDb.FullName);
        }
    }
}
