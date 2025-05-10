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
    public class CustomerServiceTest
    {
        private MaverickBankContext _context;
        private CustomerRepository _customerRepo;
        private IMapper _mapper;
        private CustomerService _customerService;
        private Mock<ILogger<CustomerService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB for each test
                .Options;

            _context = new MaverickBankContext(options);

            _context.AccountTypes.AddRange(new List<AccountType>
            {
                new AccountType { AccountTypeId = 1, AccountTypeName = "Savings" },
                new AccountType { AccountTypeId = 2, AccountTypeName = "Current" }
            });

            _context.Customers.AddRange(new List<Customer>
            {
                new Customer
                {
                    CustomerId = 1,
                    FullName = "John Doe",
                    Gender = "Male",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    AadharNumber = "123456789012",
                    PANNumber = "ABCDE1234F",
                    PhoneNumber = "9876543210",
                    Email = "johndoe@example.com",
                    Address = "123 Main St",
                    CreatedAt = DateTime.Now,
                    UserId = 1
                }
            });

            _context.Accounts.AddRange(new List<Account>
            {
                new Account
                {
                    AccountId = 1,
                    AccountNumber = "ACC123456789",
                    Balance = 1000,
                    AccountTypeId = 1,
                    CustomerId = 1
                },
                new Account
                {
                    AccountId = 2,
                    AccountNumber = "ACC987654321",
                    Balance = 500,
                    AccountTypeId = 2,
                    CustomerId = 1
                }
            });

            _context.SaveChanges();

            _customerRepo = new CustomerRepository(_context);
            _mapper = CreateMapper();
            _loggerMock = new Mock<ILogger<CustomerService>>();

            _customerService = new CustomerService(_customerRepo, _mapper, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetCustomerDetailsAsync_ShouldReturnCustomerSelfDetailsDTO_WithAccounts()
        {
            var result = await _customerService.GetCustomerDetailsAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("John Doe", result.FullName);
            Assert.AreEqual("johndoe@example.com", result.Email);
            Assert.AreEqual(2, result.Accounts.Count);

            Assert.IsTrue(result.Accounts.Any(a => a.AccountNumber == "ACC123456789" && a.Balance == 1000));
            Assert.IsTrue(result.Accounts.Any(a => a.AccountNumber == "ACC987654321" && a.Balance == 500));
        }

        [Test]
        public async Task UpdateCustomerAsync_ShouldUpdateCustomerAndReturnUpdatedDetails()
        {
            var updateDto = new CustomerUpdateDTO
            {
                FullName = "John Updated",
                PhoneNumber = "9999999999",
                Email = "updatedemail@example.com"
            };

            var result = await _customerService.UpdateCustomerAsync(1, updateDto);

            Assert.IsNotNull(result);
            Assert.AreEqual("John Updated", result.FullName);
            Assert.AreEqual("updatedemail@example.com", result.Email);

            var updatedCustomer = await _context.Customers.FindAsync(1);
            Assert.AreEqual("John Updated", updatedCustomer.FullName);
            Assert.AreEqual("updatedemail@example.com", updatedCustomer.Email);
        }

        [Test]
        public async Task GetAccountsByCustomerIdAsync_ShouldReturnListOfAccountDetailsDTO()
        {
            var result = await _customerService.GetAccountsByCustomerIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(a => a.AccountNumber == "ACC123456789" && a.Balance == 1000 && a.AccountType == "Savings"));
            Assert.IsTrue(result.Any(a => a.AccountNumber == "ACC987654321" && a.Balance == 500 && a.AccountType == "Current"));
        }

        [Test]
        public async Task GetAccountsByCustomerIdAsync_ShouldReturnEmpty_WhenNoAccountsExist()
        {
            _context.Customers.Add(new Customer
            {
                CustomerId = 2,
                FullName = "No Account User",
                Email = "noaccount@example.com",
                AadharNumber = "987654321012",
                PANNumber = "ZYXWV9876K",
                PhoneNumber = "9000000003",
                Gender = "Male",
                Address = "456 Another St",
            });
            await _context.SaveChangesAsync();

            var result = await _customerService.GetAccountsByCustomerIdAsync(2);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetCustomerDetailsAsync_ShouldMapNestedAccountsProperly()
        {
            var result = await _customerService.GetCustomerDetailsAsync(1);

            foreach (var acc in result.Accounts)
            {
                Assert.IsFalse(string.IsNullOrEmpty(acc.AccountType));
            }
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Customer, CustomerSelfDetailsDTO>();
                cfg.CreateMap<Account, CustomerAccountDTO>()
                   .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType.AccountTypeName));
                cfg.CreateMap<Account, AccountDetailsDTO>()
                   .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType.AccountTypeName));
            });
            return config.CreateMapper();
        }
    }
}

