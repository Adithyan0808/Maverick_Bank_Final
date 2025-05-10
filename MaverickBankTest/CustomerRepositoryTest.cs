using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using MaverickBank.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaverickBankTest
{
    public class CustomerRepositoryTests
    {
        private MaverickBankContext? _context;
        private CustomerRepository _customerRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MaverickBankContext(options);

            var user = new User
            {
                UserId = 1,
                Username = "customerUser",
                PasswordHash = "hashed",
                Role = "Customer" 
            };

            _context.Users.Add(user);

            var customer = new Customer
            {
                CustomerId = 1,
                FullName = "John Doe",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1),
                AadharNumber = "123456789012",
                PANNumber = "ABCDE1234F",
                PhoneNumber = "9876543210",
                Email = "john.doe@example.com",
                Address = "123 Test St",
                UserId = 1,
                User = user
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            _customerRepository = new CustomerRepository(_context);

            var seededCustomers = _context.Customers.ToList();
            Assert.That(seededCustomers.Count, Is.EqualTo(1));
            Assert.That(seededCustomers.Any(c => c.FullName == "John Doe"), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task GetAll_ShouldReturnCustomers_WhenDatabaseHasData()
        {
            var customers = await _customerRepository.GetAll();

            Assert.IsNotNull(customers);
            Assert.That(customers.Count(), Is.EqualTo(1));
            Assert.That(customers.Any(c => c.FullName == "John Doe"), Is.True);
        }

        [Test]
        public async Task GetById_ShouldReturnCorrectCustomer_WhenIdExists()
        {
            var customer = await _customerRepository.GetById(1);

            Assert.IsNotNull(customer);
            Assert.That(customer.FullName, Is.EqualTo("John Doe"));
            Assert.That(customer.Email, Is.EqualTo("john.doe@example.com"));
        }

        [Test]
        public async Task GetCustomerWithDetails_ShouldReturnCustomerWithAccounts_WhenCustomerExists()
        {
            var account = new Account
            {
                AccountId = 1,
                AccountNumber = "ACC123",
                Balance = 5000,
                CustomerId = 1
            };
            _context.Accounts.Add(account);
            _context.SaveChanges();

            var customer = await _customerRepository.GetCustomerWithDetailsAsync(1);

            Assert.IsNotNull(customer);
            Assert.That(customer.FullName, Is.EqualTo("John Doe"));
            Assert.That(customer.Accounts.Count(), Is.EqualTo(1));
            Assert.That(customer.Accounts.Any(a => a.AccountNumber == "ACC123"), Is.True);
        }

        [Test]
        public async Task UpdateCustomer_ShouldUpdateFields_WhenValidDataIsProvided()
        {
            var updateDto = new CustomerUpdateDTO
            {
                FullName = "John Updated",
                PhoneNumber = "1234567890",
                Email = "john.updated@example.com"
            };

            var updatedCustomer = await _customerRepository.UpdateCustomerAsync(1, updateDto);

            Assert.IsNotNull(updatedCustomer);
            Assert.That(updatedCustomer.FullName, Is.EqualTo("John Updated"));
            Assert.That(updatedCustomer.PhoneNumber, Is.EqualTo("1234567890"));
            Assert.That(updatedCustomer.Email, Is.EqualTo("john.updated@example.com"));
        }

        [Test]
        public async Task GetAccountsByCustomerId_ShouldReturnAccounts_WhenCustomerHasAccounts()
        {
            var account = new Account
            {
                AccountId = 1,
                AccountNumber = "ACC123",
                Balance = 5000,
                CustomerId = 1
            };
            _context.Accounts.Add(account);
            _context.SaveChanges();

            var accounts = await _customerRepository.GetAccountsByCustomerIdAsync(1);

            Assert.IsNotNull(accounts);
            Assert.That(accounts.Count(), Is.EqualTo(1));
            Assert.That(accounts.Any(a => a.AccountNumber == "ACC123"), Is.True);
        }

        [Test]
        public async Task GetById_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            var customer = await _customerRepository.GetById(999);
            Assert.IsNull(customer);
        }

        [Test]
        public void GetCustomerWithDetails_ShouldThrowException_WhenCustomerNotFound()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _customerRepository.GetCustomerWithDetailsAsync(999));
            Assert.That(ex.Message, Is.EqualTo("Customer not found"));
        }

        [Test]
        public void UpdateCustomer_ShouldThrowException_WhenCustomerDoesNotExist()
        {
            var updateDto = new CustomerUpdateDTO { FullName = "Ghost" };

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _customerRepository.UpdateCustomerAsync(999, updateDto));

            Assert.That(ex.Message, Is.EqualTo("Customer not found"));
        }


        [Test]
        public async Task UpdateCustomer_ShouldNotChangeFields_WhenDtoHasEmptyValues()
        {
            var original = await _customerRepository.GetById(1);

            var updateDto = new CustomerUpdateDTO
            {
                FullName = "",  // Empty string
                PhoneNumber = null,
                Email = " "     
            };

            var updated = await _customerRepository.UpdateCustomerAsync(1, updateDto);

            Assert.That(updated.FullName, Is.EqualTo(original.FullName));
            Assert.That(updated.PhoneNumber, Is.EqualTo(original.PhoneNumber));
            Assert.That(updated.Email, Is.EqualTo(original.Email));
        }

        [Test]
        public void GetAccountsByCustomerId_ShouldThrowException_WhenCustomerNotFound()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _customerRepository.GetAccountsByCustomerIdAsync(999));
            Assert.That(ex.Message, Is.EqualTo("Customer not found"));
        }

        [Test]
        public async Task UpdateCustomer_ShouldUpdateOnlyProvidedFields()
        {
            var updateDto = new CustomerUpdateDTO
            {
                PhoneNumber = "9999999999"
            };

            var updated = await _customerRepository.UpdateCustomerAsync(1, updateDto);

            Assert.That(updated.PhoneNumber, Is.EqualTo("9999999999"));
            Assert.That(updated.FullName, Is.EqualTo("John Doe")); 
        }

        [Test]
        public async Task UpdateCustomer_ShouldNotChange_WhenSameDataProvided()
        {
            var customerBefore = await _customerRepository.GetById(1);

            var updateDto = new CustomerUpdateDTO
            {
                FullName = customerBefore.FullName,
                Email = customerBefore.Email
            };

            var updated = await _customerRepository.UpdateCustomerAsync(1, updateDto);

            Assert.That(updated.FullName, Is.EqualTo(customerBefore.FullName));
            Assert.That(updated.Email, Is.EqualTo(customerBefore.Email));
        }

        [Test]
        public async Task GetCustomerWithDetails_ShouldReturnMultipleAccounts()
        {
            var savings = new AccountType { AccountTypeId = 1, AccountTypeName = "Savings" };
            var current = new AccountType { AccountTypeId = 2, AccountTypeName = "Current" };

            var accounts = new List<Account>
            {
                new Account { AccountId = 1, AccountNumber = "ACC001", CustomerId = 1, AccountType = savings },
                new Account { AccountId = 2, AccountNumber = "ACC002", CustomerId = 1, AccountType = current }
            };

            _context.AccountTypes.AddRange(savings, current);
            _context.Accounts.AddRange(accounts);
            _context.SaveChanges();

            var customer = await _customerRepository.GetCustomerWithDetailsAsync(1);

            Assert.That(customer.Accounts.Count(), Is.EqualTo(2));
            Assert.That(customer.Accounts.Any(a => a.AccountType.AccountTypeName == "Savings"), Is.True);
            Assert.That(customer.Accounts.Any(a => a.AccountType.AccountTypeName == "Current"), Is.True);
        }


        [Test]
        public async Task GetAll_ShouldReturnEmpty_WhenNoCustomersExist()
        {
            _context.Customers.RemoveRange(_context.Customers);
            await _context.SaveChangesAsync();

            var customers = await _customerRepository.GetAll();

            Assert.IsNotNull(customers);
            Assert.That(customers.Any(), Is.False);
        }

        [Test]
        public async Task UpdateCustomer_ShouldOverridePreviousChange()
        {
            var customer = await _customerRepository.GetById(1);
            customer.FullName = "Temporary Change";
            _context.SaveChanges();

            var updateDto = new CustomerUpdateDTO
            {
                FullName = "Final Name"
            };

            var updated = await _customerRepository.UpdateCustomerAsync(1, updateDto);

            Assert.That(updated.FullName, Is.EqualTo("Final Name"));
        }











    }
}
