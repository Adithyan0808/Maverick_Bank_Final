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
using System.Linq;

namespace MaverickBankTest.ServiceTest
{
    [TestFixture]
    public class CustomerRegistrationServiceTests
    {
        private MaverickBankContext _context;
        private CustomerRegistrationService _service;
        private Mock<ILogger<CustomerRegistrationService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            _context = new MaverickBankContext(options);

            _loggerMock = new Mock<ILogger<CustomerRegistrationService>>();

            _service = new CustomerRegistrationService(_context, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task RegisterCustomerAsync_ShouldRegisterCustomerSuccessfully()
        {
            var registerDto = new RegisterCustomerDTO
            {
                Username = "john_doe",
                Password = "Password123",
                FullName = "John Doe",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 5, 1),
                AadharNumber = "123456789012",
                PANNumber = "ABCDE1234F",
                PhoneNumber = "9876543210",
                Email = "john.doe@example.com",
                Address = "123 Main St",
                AccountNumber = "1000000001",
                AccountTypeId = 1
            };

            var result = await _service.RegisterCustomerAsync(registerDto);

            Assert.NotNull(result);
            Assert.AreEqual("john_doe", result.Username);
            Assert.AreEqual("Customer", result.Role);

            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == "john_doe");
            Assert.NotNull(userInDb);

            var customerInDb = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userInDb.UserId);
            Assert.NotNull(customerInDb);
        }

        [Test]
        public async Task RegisterCustomerAsync_ShouldThrowException_WhenUsernameAlreadyExists()
        {
            var existingUser = new User
            {
                Username = "john_doe",
                PasswordHash = "hashedPassword",
                Role = "Customer"
            };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var registerDto = new RegisterCustomerDTO
            {
                Username = "john_doe",
                Password = "Password123",
                FullName = "Jane Doe",
                Gender = "Female",
                DateOfBirth = new DateTime(1992, 6, 1),
                AadharNumber = "987654321098",
                PANNumber = "XYZAB1234T",
                PhoneNumber = "9876543210",
                Email = "jane.doe@example.com",
                Address = "456 Elm St",
                AccountNumber = "1000000002",
                AccountTypeId = 1
            };

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.RegisterCustomerAsync(registerDto));
            Assert.AreEqual("Username already exists", ex.Message);
        }

        [Test]
        public async Task RegisterCustomerAsync_ShouldCreateCustomerWithAccount()
        {
            var registerDto = new RegisterCustomerDTO
            {
                Username = "alice_smith",
                Password = "SecurePassword123",
                FullName = "Alice Smith",
                Gender = "Female",
                DateOfBirth = new DateTime(1985, 8, 10),
                AadharNumber = "112233445566",
                PANNumber = "XYZAB6789L",
                PhoneNumber = "9876543211",
                Email = "alice.smith@example.com",
                Address = "789 Oak St",
                AccountNumber = "1000000003",
                AccountTypeId = 1
            };

            var result = await _service.RegisterCustomerAsync(registerDto);

            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == "alice_smith");
            var customerInDb = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userInDb.UserId);
            var accountInDb = await _context.Accounts.FirstOrDefaultAsync(a => a.CustomerId == customerInDb.CustomerId);

            Assert.NotNull(userInDb);
            Assert.NotNull(customerInDb);
            Assert.NotNull(accountInDb);
        }
    }
}
