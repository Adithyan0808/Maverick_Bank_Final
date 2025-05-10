//using AutoMapper;
//using MaverickBank.Contexts;
//using MaverickBank.Models.DTOs;
//using MaverickBank.Services;
//using Microsoft.EntityFrameworkCore;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MaverickBank.Tests.Services
//{
//    [TestFixture]
//    public class AccountTypeServiceTests
//    {
//        private MaverickBankContext _context;
//        private IMapper _mapper;
//        private AccountTypeService _accountTypeService;

//        [SetUp]
//        public void Setup()
//        {
//            var options = new DbContextOptionsBuilder<MaverickBankContext>()
//                .UseInMemoryDatabase(databaseName: "MaverickBankContext")
//                .Options;

//            _context = new MaverickBankContext(options);

//            _context.AccountTypes.AddRange(new List<AccountType>
//            {
//                new AccountType { AccountTypeId = 1, AccountTypeName = "Savings" },
//                new AccountType { AccountTypeId = 2, AccountTypeName = "Current" }
//            });

//            _context.SaveChanges();

//            var config = new MapperConfiguration(cfg =>
//            {
//                cfg.CreateMap<AccountType, AccountTypeDto>();
//            });

//            _mapper = config.CreateMapper();
//            _accountTypeService = new AccountTypeService(_context, _mapper);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _context.Database.EnsureDeleted();
//            _context.Dispose();
//        }

//        [Test]
//        public async Task GetAllAccountTypesAsync_ShouldReturnAllAccountTypes()
//        {
//            var result = await _accountTypeService.GetAllAccountTypesAsync();

//            Assert.IsNotNull(result);
//            Assert.AreEqual(2, result.AccountTypes.Count);
//            Assert.IsTrue(result.AccountTypes.Any(at => at.AccountTypeName == "Savings"));
//            Assert.IsTrue(result.AccountTypes.Any(at => at.AccountTypeName == "Current"));
//        }

//        [Test]
//        public async Task GetAllAccountTypesAsync_ShouldReturnEmpty_WhenNoAccountTypesExist()
//        {
//            _context.AccountTypes.RemoveRange(_context.AccountTypes);
//            await _context.SaveChangesAsync();

//            var result = await _accountTypeService.GetAllAccountTypesAsync();


//            Assert.IsNotNull(result);
//            Assert.AreEqual(0, result.AccountTypes.Count); 
//        }
//    }
//}




using AutoMapper;
using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using MaverickBank.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaverickBank.Tests.Services
{
    [TestFixture]
    public class AccountTypeServiceTests
    {
        private MaverickBankContext _context;
        private IMapper _mapper;
        private AccountTypeService _accountTypeService;
        private Mock<ILogger<AccountTypeService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: "MaverickBankContext")
                .Options;

            _context = new MaverickBankContext(options);

            _context.AccountTypes.AddRange(new List<AccountType>
            {
                new AccountType { AccountTypeId = 1, AccountTypeName = "Savings" },
                new AccountType { AccountTypeId = 2, AccountTypeName = "Current" }
            });

            _context.SaveChanges();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AccountType, AccountTypeDto>();
            });

            _mapper = config.CreateMapper();
            _loggerMock = new Mock<ILogger<AccountTypeService>>();

            _accountTypeService = new AccountTypeService(_context, _mapper, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAccountTypesAsync_ShouldReturnAllAccountTypes()
        {
            var result = await _accountTypeService.GetAllAccountTypesAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.AccountTypes.Count);
            Assert.IsTrue(result.AccountTypes.Any(at => at.AccountTypeName == "Savings"));
            Assert.IsTrue(result.AccountTypes.Any(at => at.AccountTypeName == "Current"));

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetching all account types")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetAllAccountTypesAsync_ShouldReturnEmpty_WhenNoAccountTypesExist()
        {
            _context.AccountTypes.RemoveRange(_context.AccountTypes);
            await _context.SaveChangesAsync();

            var result = await _accountTypeService.GetAllAccountTypesAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.AccountTypes.Count);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetching all account types")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}

