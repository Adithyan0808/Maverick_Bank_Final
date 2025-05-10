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
    public class AdminServiceTest
    {
        private MaverickBankContext _context;
        private AdminRepository _adminRepo;
        private IMapper _mapper;
        private AdminService _adminService;
        private Mock<ILogger<AdminService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ensure isolation between tests
                .Options;

            _context = new MaverickBankContext(options);
            _context.Database.EnsureCreated();

            ResetData();

            _adminRepo = new AdminRepository(_context);
            _mapper = CreateMapper();
            _loggerMock = new Mock<ILogger<AdminService>>();
            _adminService = new AdminService(_adminRepo, _mapper, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void ResetData()
        {
            _context.Employees.RemoveRange(_context.Employees);
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            _context.Employees.AddRange(new List<Employee>
            {
                new Employee { EmployeeId = 1, FullName = "Employee1", Email = "employee1@example.com", PhoneNumber = "12345678", Branch = new Branch { BranchId = 1, BranchName = "Branch1", Location = "TestLocation1" } },
                new Employee { EmployeeId = 2, FullName = "Employee2", Email = "employee2@example.com", PhoneNumber = "87654321", Branch = new Branch { BranchId = 2, BranchName = "Branch2", Location = "TestLocation2" } }
            });

            _context.SaveChanges();
        }

        [Test]
        public async Task GetAllEmployeesAsync_ShouldReturnListOfEmployeeDetailsDTO()
        {
            var result = await _adminService.GetAllEmployeesAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(e => e.FullName == "Employee1"));
            Assert.IsTrue(result.Any(e => e.FullName == "Employee2"));
        }

        [Test]
        public async Task GetEmployeeByIdAsync_ShouldReturnEmployeeDetailsDTO()
        {
            var result = await _adminService.GetEmployeeByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Employee1", result.FullName);
            Assert.AreEqual("employee1@example.com", result.Email);
        }

        [Test]
        public async Task UpdateEmployeeDetailsAsync_ShouldUpdateAndReturnUpdatedEmployeeDTO()
        {
            var updateDto = new EmployeeUpdateDTO
            {
                FullName = "Updated Name",
                PhoneNumber = "1234567890",
                Email = "updatedemail@example.com"
            };

            var result = await _adminService.UpdateEmployeeDetailsAsync(1, updateDto);

            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Name", result.FullName);
            Assert.AreEqual("updatedemail@example.com", result.Email);
            Assert.AreEqual("1234567890", result.PhoneNumber);

            var updatedEmployee = await _context.Employees.FindAsync(1);
            Assert.AreEqual("Updated Name", updatedEmployee.FullName);
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Employee, EmployeeDetailsDTO>();
                cfg.CreateMap<Employee, EmployeeDTO>();
            });
            return config.CreateMapper();
        }
    }
}
