using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using MaverickBank.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MaverickBankTest.ServiceTest
{
    [TestFixture]
    public class TransactionTypeServiceTest
    {
        private MaverickBankContext _context;
        private TransactionTypeService _transactionTypeService;
        private IMapper _mapper;
        private Mock<ILogger<TransactionTypeService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: "TransactionTypeTestDb")
                .Options;

            _context = new MaverickBankContext(options);

            _context.TransactionTypes.AddRange(new List<TransactionType>
            {
                new TransactionType { TransactionTypeId = 1, TransactionTypeName = "Deposit" },
                new TransactionType { TransactionTypeId = 2, TransactionTypeName = "Withdrawal" },
                new TransactionType { TransactionTypeId = 3, TransactionTypeName = "Transfer" }
            });

            _context.SaveChanges();

            _mapper = CreateMapper();
            _mockLogger = new Mock<ILogger<TransactionTypeService>>();
            _transactionTypeService = new TransactionTypeService(_context, _mapper, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllTransactionTypesAsync_ShouldReturnAllTypesInDropdownFormat()
        {
            // Act
            var result = await _transactionTypeService.GetAllTransactionTypesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TransactionTypes);
            Assert.AreEqual(3, result.TransactionTypes.Count);
            Assert.AreEqual("Deposit", result.TransactionTypes[0].TransactionTypeName);
            Assert.AreEqual("Withdrawal", result.TransactionTypes[1].TransactionTypeName);
            Assert.AreEqual("Transfer", result.TransactionTypes[2].TransactionTypeName);

            // Verify logger calls
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetching all transaction types")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Mapped 3 transaction types")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TransactionType, TransactionTypeDto>();
            });

            return config.CreateMapper();
        }
    }
}
