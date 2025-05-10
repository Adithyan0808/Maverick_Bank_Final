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
    public class EmployeeRepositoryTests
    {
        private MaverickBankContext? _context;
        private EmployeeRepository _employeeRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MaverickBankContext(options);

            var branch = new Branch
            {
                BranchId = 1,
                BranchName = "Test Branch",
                Location = "Test Location"
            };

            var user = new User
            {
                UserId = 1,
                Username = "employeeUser",
                PasswordHash = "hashed",
                Role = "Employee"
            };

            _context.Branches.Add(branch);
            _context.Users.Add(user);

            var employee = new Employee
            {
                EmployeeId = 1,
                FullName = "John Smith",
                PhoneNumber = "1234567890",
                Email = "john.smith@example.com",
                BranchId = 1,
                UserId = 1,
                Branch = branch,
                User = user
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();

            _employeeRepository = new EmployeeRepository(_context);

            var seededEmployees = _context.Employees.ToList();
            Assert.That(seededEmployees.Count, Is.EqualTo(1));
            Assert.That(seededEmployees.Any(e => e.FullName == "John Smith"), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task GetAll_ShouldReturnEmployees_WhenDatabaseHasData()
        {
            var employees = await _employeeRepository.GetAll();

            Assert.IsNotNull(employees);
            Assert.That(employees.Count(), Is.EqualTo(1));
            Assert.That(employees.Any(e => e.FullName == "John Smith"), Is.True);
        }

        [Test]
        public async Task GetById_ShouldReturnCorrectEmployee_WhenIdExists()
        {
            var employee = await _employeeRepository.GetById(1);

            Assert.IsNotNull(employee);
            Assert.That(employee.FullName, Is.EqualTo("John Smith"));
            Assert.That(employee.Email, Is.EqualTo("john.smith@example.com"));
        }

        [Test]
        public async Task GetAllCustomers_ShouldReturnCustomers_WhenCustomersExist()
        {
            var customer = new Customer
            {
                FullName = "Test Customer",
                PhoneNumber = "1234567890",
                Email = "test@example.com",
                AadharNumber = "123412341234",
                PANNumber = "ABCDE1234F",
                Gender = "Male",
                Address = "123 Street",
                DateOfBirth = new DateTime(1995, 1, 1),
                UserId = 1
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var customers = await _employeeRepository.GetAllCustomersAsync();

            Assert.IsNotNull(customers);
            Assert.That(customers.Count(), Is.EqualTo(1));
            Assert.That(customers.Any(c => c.FullName == "Test Customer"), Is.True);
        }

        [Test]
        public async Task GetCustomerById_ShouldReturnCustomer_WhenIdExists()
        {
            var customer = new Customer
            {
                FullName = "Test Customer",
                PhoneNumber = "1234567890",
                Email = "test@example.com",
                AadharNumber = "123412341234",
                PANNumber = "ABCDE1234F",
                Gender = "Male",
                Address = "123 Street",
                DateOfBirth = new DateTime(1995, 1, 1),
                UserId = 1,
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var retrievedCustomer = await _employeeRepository.GetCustomerByIdAsync(1);

            Assert.IsNotNull(retrievedCustomer);
            Assert.That(retrievedCustomer.FullName, Is.EqualTo("Test Customer"));
        }

        [Test]
        public async Task GetCustomersByAccountType_ShouldReturnCustomers_WhenAccountTypeMatches()
        {
            var accountType = new AccountType
            {
                AccountTypeId = 1,
                AccountTypeName = "Savings"
            };
            var account = new Account
            {
                AccountId = 1,
                AccountNumber = "ACC123",
                Balance = 5000,
                AccountTypeId = 1,
                CustomerId = 1
            };
            var customer = new Customer
            {
                FullName = "Test Customer",
                PhoneNumber = "1234567890",
                Email = "test@example.com",
                AadharNumber = "123412341234",
                PANNumber = "ABCDE1234F",
                Gender = "Male",
                Address = "123 Street",
                DateOfBirth = new DateTime(1995, 1, 1),
                UserId = 1,
                Accounts = new List<Account> { account }
            };
            _context.AccountTypes.Add(accountType);
            _context.Accounts.Add(account);
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var customers = await _employeeRepository.GetCustomersByAccountTypeAsync("Savings");

            Assert.IsNotNull(customers);
            Assert.That(customers.Count(), Is.EqualTo(1));
            Assert.That(customers.Any(c => c.FullName == "Test Customer"), Is.True);
        }

        [Test]
        public async Task DeleteCustomerById_ShouldDeleteCustomer_WhenCustomerExists()
        {
            var customer = new Customer
            {
                FullName = "Test Customer",
                PhoneNumber = "1234567890",
                Email = "test@example.com",
                AadharNumber = "123412341234",
                PANNumber = "ABCDE1234F",
                Gender = "Male",
                Address = "123 Street",
                DateOfBirth = new DateTime(1995, 1, 1),
                UserId = 1,
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var result = await _employeeRepository.DeleteCustomerByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.That(result.GetType().GetProperty("message")?.GetValue(result), Is.EqualTo("deleted successfully"));
        }


        [Test]
        public void GetAll_ShouldThrowException_WhenNoEmployeesExist()
        {
            _context.Employees.RemoveRange(_context.Employees);
            _context.SaveChanges();

            var ex = Assert.ThrowsAsync<Exception>(async () => await _employeeRepository.GetAll());
            Assert.That(ex.Message, Is.EqualTo("No employees found"));
        }

        [Test]
        public void GetById_ShouldThrowException_WhenEmployeeNotFound()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _employeeRepository.GetById(999));
            Assert.That(ex.Message, Is.EqualTo("Employee not found"));
        }

        [Test]
        public void GetAllCustomersAsync_ShouldThrowException_WhenNoCustomersExist()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _employeeRepository.GetAllCustomersAsync());
            Assert.That(ex.Message, Is.EqualTo("No customers found"));
        }


        [Test]
        public void GetCustomerByIdAsync_ShouldThrowException_WhenCustomerNotFound()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _employeeRepository.GetCustomerByIdAsync(999));
            Assert.That(ex.Message, Is.EqualTo("Customer not found"));
        }

        [Test]
        public void GetCustomersByAccountTypeAsync_ShouldThrowException_WhenNoMatch()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _employeeRepository.GetCustomersByAccountTypeAsync("NonExistentType"));
            Assert.That(ex.Message, Is.EqualTo("No customers found for the given account type"));
        }


        [Test]
        public async Task UpdateCustomerAsync_ShouldUpdateOnlySpecifiedFields()
        {
            var customer = new Customer
            {
                CustomerId = 1,
                FullName = "Old Name",
                Gender = "Male",
                PhoneNumber = "1111111111",
                Email = "old@example.com",
                Address = "Old Address",
                AadharNumber = "123456789012",
                PANNumber = "ABCDE1234F",
                DateOfBirth = new DateTime(1990, 1, 1),
                UserId = 1
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var updateDto = new CustomerUpdateDTO
            {
                FullName = "New Name",
                Email = "new@example.com"
            };

            var updatedCustomer = await _employeeRepository.UpdateCustomerAsync(1, updateDto);

            Assert.That(updatedCustomer.FullName, Is.EqualTo("New Name"));
            Assert.That(updatedCustomer.Email, Is.EqualTo("new@example.com"));
            Assert.That(updatedCustomer.PhoneNumber, Is.EqualTo("1111111111")); // unchanged
        }

        [Test]
        public void UpdateCustomerAsync_ShouldThrowException_WhenCustomerNotFound()
        {
            var updateDto = new CustomerUpdateDTO { FullName = "Test" };

            var ex = Assert.ThrowsAsync<Exception>(async () => await _employeeRepository.UpdateCustomerAsync(999, updateDto));
            Assert.That(ex.Message, Is.EqualTo("Customer not found"));
        }


        [Test]
        public async Task DeleteCustomerByIdAsync_ShouldAlsoRemoveUser_WhenUserExists()
        {
            var user = new User
            {
                UserId = 2,
                Username = "testuser",
                PasswordHash = "hashed",
                Role = "Customer"
            };

            var customer = new Customer
            {
                CustomerId = 2,
                FullName = "Deletable Customer",
                PhoneNumber = "1111111111",
                Email = "delete@example.com",
                AadharNumber = "999988887777",
                PANNumber = "ZZZZZ9999Z",
                Gender = "Other",
                Address = "Test Addr",
                DateOfBirth = new DateTime(1992, 1, 1),
                UserId = 2,
                User = user
            };

            _context.Users.Add(user);
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var result = await _employeeRepository.DeleteCustomerByIdAsync(2);

            var deletedCustomer = _context.Customers.Find(2);
            var deletedUser = _context.Users.Find(2);

            Assert.IsNull(deletedCustomer);
            Assert.IsNull(deletedUser);
            Assert.That(result.GetType().GetProperty("message")?.GetValue(result), Is.EqualTo("deleted successfully"));
        }


        [Test]
        public void DeleteCustomerByIdAsync_ShouldThrowException_WhenCustomerNotFound()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _employeeRepository.DeleteCustomerByIdAsync(999));
            Assert.That(ex.Message, Is.EqualTo("Customer not found"));
        }

        [Test]
        public async Task GetById_ShouldReturnBranchDetails_WhenEmployeeExists()
        {
            var employee = await _employeeRepository.GetById(1);

            Assert.IsNotNull(employee.Branch);
            Assert.That(employee.Branch.BranchName, Is.EqualTo("Test Branch"));
        }

        [Test]
        public async Task GetAll_ShouldIncludeBranchInfo_ForEachEmployee()
        {
            var employees = await _employeeRepository.GetAll();

            foreach (var employee in employees)
            {
                Assert.IsNotNull(employee.Branch);
                Assert.That(employee.Branch.BranchName, Is.EqualTo("Test Branch"));
            }
        }


        [Test]
        public async Task UpdateCustomerAsync_ShouldNotUpdateFields_WhenDTOIsEmpty()
        {
            var customer = new Customer
            {
                CustomerId = 3,
                FullName = "Original Name",
                Gender = "Female",
                PhoneNumber = "2222222222",
                AadharNumber = "123456789012",
                PANNumber = "ABCDE1234F",
                Email = "original@example.com",
                Address = "Original Addr",
                DateOfBirth = new DateTime(1991, 1, 1),
                UserId = 1
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var emptyDto = new CustomerUpdateDTO(); // all null/empty

            var updatedCustomer = await _employeeRepository.UpdateCustomerAsync(3, emptyDto);

            Assert.That(updatedCustomer.FullName, Is.EqualTo("Original Name"));
            Assert.That(updatedCustomer.Email, Is.EqualTo("original@example.com"));
        }

        [Test]
        public async Task GetCustomerByIdAsync_ShouldIncludeAccountTypes()
        {
            var accountType = new AccountType
            {
                AccountTypeId = 2,
                AccountTypeName = "Current"
            };
            var account = new Account
            {
                AccountId = 2,
                AccountNumber = "CURR001",
                Balance = 7000,
                AccountTypeId = 2,
                AccountType = accountType,
                CustomerId = 4
            };
            var customer = new Customer
            {
                CustomerId = 4,
                FullName = "Type Test Customer",
                PhoneNumber = "3333333333",
                Email = "type@example.com",
                AadharNumber = "111122223333",
                PANNumber = "PPPPP1234P",
                Gender = "Male",
                Address = "Typed Address",
                DateOfBirth = new DateTime(1985, 2, 2),
                UserId = 1,
                Accounts = new List<Account> { account }
            };
            _context.AccountTypes.Add(accountType);
            _context.Accounts.Add(account);
            _context.Customers.Add(customer);
            _context.SaveChanges();

            var result = await _employeeRepository.GetCustomerByIdAsync(4);

            Assert.IsNotNull(result.Accounts.First().AccountType);
            Assert.That(result.Accounts.First().AccountType.AccountTypeName, Is.EqualTo("Current"));
        }





    }
}
