using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using MaverickBank.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MaverickBankTest
{
    public class AdminRepositoryTests
    {
        private MaverickBankContext? _context;
        private AdminRepository _adminRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MaverickBankContext(options);

            var user = new User { UserId = 1, Username = "adminUser", PasswordHash = "hashed", Role = "Admin" };
            var branch = new Branch { BranchId = 1, BranchName = "Main Branch", Location = "Downtown" };

            _context.Users.Add(user);
            _context.Branches.Add(branch);
            _context.Admins.Add(new Admin
            {
                AdminId = 1,
                FullName = "Admin One",
                Email = "admin@bank.com",
                UserId = 1,
                User = user
            });

            _context.Employees.Add(new Employee
            {
                EmployeeId = 1,
                FullName = "Emp One",
                Email = "emp1@bank.com",
                PhoneNumber = "1234567890",
                BranchId = 1,
                Branch = branch,
                User = user
            });

            _context.SaveChanges();

            _adminRepository = new AdminRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task GetAll_ShouldReturnAdmins_WhenAdminsExist()
        {
            var result = await _adminRepository.GetAll();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().FullName, Is.EqualTo("Admin One"));
        }

        [Test]
        public async Task GetById_ShouldReturnAdmin_WhenIdExists()
        {
            var admin = await _adminRepository.GetById(1);

            Assert.IsNotNull(admin);
            Assert.That(admin.Email, Is.EqualTo("admin@bank.com"));
        }

        [Test]
        public async Task GetEmployeeByIdAsync_ShouldReturnEmployee_WhenIdExists()
        {
            var emp = await _adminRepository.GetEmployeeByIdAsync(1);

            Assert.IsNotNull(emp);
            Assert.That(emp.FullName, Is.EqualTo("Emp One"));
        }

        [Test]
        public void GetEmployeeByIdAsync_ShouldThrowException_WhenIdDoesNotExist()
        {
            var ex = Assert.ThrowsAsync<Exception>(() => _adminRepository.GetEmployeeByIdAsync(99));
            Assert.That(ex.Message, Is.EqualTo("Employee not found"));
        }

        [Test]
        public async Task UpdateEmployeeDetailsAsync_ShouldUpdateFields_WhenDtoIsValid()
        {
            var dto = new EmployeeUpdateDTO
            {
                FullName = "Updated Name",
                Email = "updated@bank.com",
                PhoneNumber = "9876543210"
            };

            var updated = await _adminRepository.UpdateEmployeeDetailsAsync(1, dto);

            Assert.That(updated.FullName, Is.EqualTo("Updated Name"));
            Assert.That(updated.Email, Is.EqualTo("updated@bank.com"));
            Assert.That(updated.PhoneNumber, Is.EqualTo("9876543210"));
        }

        [Test]
        public async Task DeleteEmployeeByIdAsync_ShouldDeleteEmployeeAndReturnResponse_WhenEmployeeExists()
        {
            var result = await _adminRepository.DeleteEmployeeByIdAsync(1);

            Assert.That(result.GetType().GetProperty("message")?.GetValue(result), Is.EqualTo("deleted successfully"));

            var emp = await _context.Employees.FindAsync(1);
            var user = await _context.Users.FindAsync(1);

            Assert.IsNull(emp);
            Assert.IsNull(user);
        }


        [Test]
        public async Task GetAllEmployeesAsync_ShouldReturnEmployees_WhenEmployeesExist()
        {
            var result = await _adminRepository.GetAllEmployeesAsync();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().FullName, Is.EqualTo("Emp One"));
        }


        [Test]
        public void GetAllEmployeesAsync_ShouldThrowException_WhenNoEmployeesExist()
        {
            _context.Employees.RemoveRange(_context.Employees);
            _context.SaveChanges();

            var ex = Assert.ThrowsAsync<Exception>(() => _adminRepository.GetAllEmployeesAsync());
            Assert.That(ex.Message, Is.EqualTo("No employees found"));
        }


        [Test]
        public void UpdateEmployeeDetailsAsync_ShouldThrowException_WhenEmployeeDoesNotExist()
        {
            var dto = new EmployeeUpdateDTO
            {
                FullName = "Ghost",
                Email = "ghost@bank.com",
                PhoneNumber = "0000000000"
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _adminRepository.UpdateEmployeeDetailsAsync(99, dto));
            Assert.That(ex.Message, Is.EqualTo("Employee not found"));
        }

        [Test]
        public void DeleteEmployeeByIdAsync_ShouldThrowException_WhenEmployeeDoesNotExist()
        {
            var ex = Assert.ThrowsAsync<Exception>(() => _adminRepository.DeleteEmployeeByIdAsync(99));
            Assert.That(ex.Message, Is.EqualTo("Employee not found"));
        }

        [Test]
        public async Task GetById_ShouldReturnNull_WhenAdminDoesNotExist()
        {
            var result = await _adminRepository.GetById(999);

            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateEmployeeDetailsAsync_ShouldNotChangeFields_WhenDtoIsEmpty()
        {
            var original = await _adminRepository.GetEmployeeByIdAsync(1);

            var dto = new EmployeeUpdateDTO(); // All fields null/empty
            var updated = await _adminRepository.UpdateEmployeeDetailsAsync(1, dto);

            Assert.That(updated.FullName, Is.EqualTo(original.FullName));
            Assert.That(updated.Email, Is.EqualTo(original.Email));
            Assert.That(updated.PhoneNumber, Is.EqualTo(original.PhoneNumber));
        }

        [Test]
        public async Task DeleteEmployeeByIdAsync_ShouldRemoveOnlyTargetEmployee_WhenMultipleExist()
        {
            // Add another employee
            _context.Employees.Add(new Employee
            {
                EmployeeId = 2,
                FullName = "Second Emp",
                Email = "emp2@bank.com",
                PhoneNumber = "2222222222",
                BranchId = 1,
                User = new User { UserId = 2, Username = "emp2", PasswordHash = "pass2", Role = "Employee" }
            });
            await _context.SaveChangesAsync();

            // Delete first employee
            await _adminRepository.DeleteEmployeeByIdAsync(1);

            var remaining = await _adminRepository.GetEmployeeByIdAsync(2);

            Assert.That(remaining.FullName, Is.EqualTo("Second Emp"));
        }


        [Test]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoAdminsExist()
        {
            _context.Admins.RemoveRange(_context.Admins);
            await _context.SaveChangesAsync();

            var result = await _adminRepository.GetAll();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(0));
        }





    }
}
