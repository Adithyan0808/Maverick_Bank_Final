using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using MaverickBank.Repositories;
using MaverickBank.Services;
using NUnit.Framework;

namespace MaverickBankTest.ServiceTest
{
    [TestFixture]
    public class EmployeeServiceTest
    {
        private MaverickBankContext _context;
        private EmployeeRepository _employeeRepo;
        private IMapper _mapper;
        private EmployeeService _employeeService;
        private Mock<ILogger<EmployeeService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB for each test
                .Options;

            _context = new MaverickBankContext(options);

            _context.AccountTypes.Add(new AccountType { AccountTypeId = 1, AccountTypeName = "Savings" });
            _context.Customers.Add(new Customer
            {
                CustomerId = 1,
                FullName = "John Doe",
                Email = "johndoe@example.com",
                AadharNumber = "123456789012",
                PANNumber = "ABCDE1234F",
                PhoneNumber = "9876543210",
                Gender = "Male",
                Address = "123 Main St",
                CreatedAt = DateTime.Now,
                UserId = 1
            });

            _context.Accounts.Add(new Account
            {
                AccountId = 1,
                AccountNumber = "ACC123456789",
                Balance = 1000,
                AccountTypeId = 1,
                CustomerId = 1
            });

            _context.SaveChanges();

            _employeeRepo = new EmployeeRepository(_context);
            _mapper = CreateMapper();
            _loggerMock = new Mock<ILogger<EmployeeService>>();
            _employeeService = new EmployeeService(_employeeRepo, _mapper, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetCustomerByIdAsync_ShouldReturnCorrectCustomer()
        {
            // Setup logger to verify the logs
            _loggerMock.Setup(log => log.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()))
                .Verifiable("Log was not called as expected.");

            var result = await _employeeService.GetCustomerByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("johndoe@example.com", result.Email);

            // Verify that the log was called with the correct parameters
            _loggerMock.Verify(
                log => log.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetching customer with ID: 1")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ), Times.Once);

            _loggerMock.Verify(
                log => log.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetched customer with ID: 1.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ), Times.Once);
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Customer, CustomerDetailsDTO>();
                cfg.CreateMap<Account, AccountDTO>()
                    .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType.AccountTypeName));
            });
            return config.CreateMapper();
        }
    }
}
