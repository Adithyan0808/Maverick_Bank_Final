using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaverickBankTest
{
    public class AccountRepositoryTests
    {
        private MaverickBankContext? _context;
        private AccountRepository _accountRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new MaverickBankContext(options);

            _context.Accounts.AddRange(new List<Account>
            {
                new Account
                {
                    AccountId = 1,
                    AccountNumber = "ACC1001",
                    Balance = 5000,
                    Customer = new Customer
                    {
                        CustomerId = 1,
                        FullName = "Alice Johnson",
                        Email = "alice@example.com",
                        AadharNumber = "111122223333",
                        PANNumber = "ABCDE1234F",
                        PhoneNumber = "9000000001",
                        Address = "101 Main St",
                        Gender = "Female"
                    }
                },
                new Account
                {
                    AccountId = 2,
                    AccountNumber = "ACC1002",
                    Balance = 10000,
                    Customer = new Customer
                    {
                        CustomerId = 2,
                        FullName = "Bob Smith",
                        Email = "bob@example.com",
                        AadharNumber = "444455556666",
                        PANNumber = "XYZAB9876K",
                        PhoneNumber = "9000000002",
                        Address = "202 Second St",
                        Gender = "Male"
                    }
                }
            });

            _context.SaveChanges();

            _accountRepository = new AccountRepository(_context);

            var seeded = _context.Accounts.Include(a => a.Customer).ToList();
            Assert.That(seeded.Count, Is.EqualTo(2));
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task GetAll_ShouldReturnAllAccounts_WithCustomers()
        {
            var result = await _accountRepository.GetAll();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(a => a.Customer != null), Is.True);
        }

        [Test]
        public async Task GetById_ShouldReturnCorrectAccount_WhenIdExists()
        {
            var result = await _accountRepository.GetById(1);

            Assert.IsNotNull(result);
            Assert.That(result.AccountNumber, Is.EqualTo("ACC1001"));
            Assert.That(result.Customer.FullName, Is.EqualTo("Alice Johnson"));
        }

        [Test]
        public void GetById_ShouldThrowException_WhenIdDoesNotExist()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _accountRepository.GetById(999);
            });

            Assert.That(ex.Message, Is.EqualTo("Account not found"));
        }

        [Test]
        public async Task GetByAccountNumberAsync_ShouldReturnCorrectAccount_WhenAccountNumberExists()
        {
            var result = await _accountRepository.GetByAccountNumberAsync("ACC1002");

            Assert.IsNotNull(result);
            Assert.That(result.AccountId, Is.EqualTo(2));
            Assert.That(result.Customer.Email, Is.EqualTo("bob@example.com"));
        }

        [Test]
        public void GetByAccountNumberAsync_ShouldThrowException_WhenAccountNumberDoesNotExist()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _accountRepository.GetByAccountNumberAsync("NONEXISTENT");
            });

            Assert.That(ex.Message, Is.EqualTo("Account not found with number: NONEXISTENT"));
        }


        [Test]
        public async Task GetById_ShouldThrowException_WhenAccountDoesNotExist()
        {
           
            var service = new AccountRepository(_context);

            // ID that does not exist in the database
            var nonExistentAccountId = 999;

            var exception = Assert.ThrowsAsync<Exception>(async () => await service.GetById(nonExistentAccountId));
            Assert.AreEqual("Account not found", exception.Message);
        }







    }
}
