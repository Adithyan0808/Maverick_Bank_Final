using Moq;
using NUnit.Framework;
using AutoMapper;
using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using MaverickBank.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MaverickBankTest
{
    [TestFixture]
    public class BranchServiceTest
    {
        private Mock<IMapper> _mockMapper;
        private MaverickBankContext _context;
        private BranchService _branchService;
        private Mock<ILogger<BranchService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            _context = new MaverickBankContext(options);

            SeedDatabase();

            _mockMapper = new Mock<IMapper>();

            var mockBranchDtos = new List<BranchDto>
            {
                new BranchDto { BranchId = 1, BranchName = "Branch 1" },
                new BranchDto { BranchId = 2, BranchName = "Branch 2" }
            };

            _mockMapper.Setup(m => m.Map<List<BranchDto>>(It.IsAny<List<Branch>>()))
                .Returns(mockBranchDtos);

            _loggerMock = new Mock<ILogger<BranchService>>();

            _branchService = new BranchService(_context, _mockMapper.Object, _loggerMock.Object);
        }

        private void SeedDatabase()
        {
            var branches = new List<Branch>
            {
                new Branch { BranchId = 1, BranchName = "Branch 1", Location = "Location 1" },
                new Branch { BranchId = 2, BranchName = "Branch 2", Location = "Location 2" }
            };

            _context.Branches.AddRange(branches);
            _context.SaveChanges();
        }

        [Test]
        public async Task GetAllBranchesAsync_ShouldReturnBranchDropdownDto()
        {
            _loggerMock.Object.LogInformation("Starting GetAllBranchesAsync test.");

            var result = await _branchService.GetAllBranchesAsync();

            _loggerMock.Object.LogInformation("GetAllBranchesAsync test completed with {BranchCount} branches fetched.", result.Branches.Count);

            Assert.NotNull(result);
            Assert.IsInstanceOf<BranchDropdownDto>(result);
            Assert.AreEqual(2, result.Branches.Count);
            Assert.AreEqual("Branch 1", result.Branches[0].BranchName);
            Assert.AreEqual("Branch 2", result.Branches[1].BranchName);
        }

        [Test]
        public async Task GetAllBranchDetailsAsync_ShouldReturnListOfBranchDetailsDto()
        {
            _loggerMock.Object.LogInformation("Starting GetAllBranchDetailsAsync test.");

            var mockBranchDetails = new List<BranchDetailsDto>
            {
                new BranchDetailsDto { BranchId = 1, BranchName = "Branch 1", Location = "Location 1" },
                new BranchDetailsDto { BranchId = 2, BranchName = "Branch 2", Location = "Location 2" }
            };

            _mockMapper.Setup(m => m.Map<List<BranchDetailsDto>>(It.IsAny<List<Branch>>()))
                .Returns(mockBranchDetails);

            var result = await _branchService.GetAllBranchDetailsAsync();

            _loggerMock.Object.LogInformation("GetAllBranchDetailsAsync test completed with {BranchDetailsCount} branch details fetched.", result.Count);

            Assert.NotNull(result);
            Assert.IsInstanceOf<List<BranchDetailsDto>>(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Location 1", result[0].Location);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}





