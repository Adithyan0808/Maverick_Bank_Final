using AutoMapper;
using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Repositories;
using MaverickBank.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;
using MaverickBank.Misc;

namespace MaverickBankTest.ServiceTest
{
    [TestFixture]
    public class LoanServiceTest
    {
        private LoanService _loanService;
        private MaverickBankContext _context;
        private IMapper _mapper;
        private Mock<ILogger<LoanService>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new MaverickBankContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new LoanMapper());
            });
            _mapper = config.CreateMapper();

            var loanRepository = new LoanRepository(_context);

            // Mocking the logger
            _mockLogger = new Mock<ILogger<LoanService>>();

            // Pass the mocked logger to the LoanService
            _loanService = new LoanService(loanRepository, _mapper, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllLoanMastersAsync_ShouldReturnAllLoanMasters()
        {
            var loanMasters = new List<LoanMaster>
            {
                new LoanMaster { LoanTypeName = "HomeLoan", MaxLoanAmount = 500000, DefaultInterestRate = 5.5f },
                new LoanMaster { LoanTypeName = "CarLoan", MaxLoanAmount = 200000, DefaultInterestRate = 7.5f }
            };

            _context.LoanMasters.AddRange(loanMasters);
            await _context.SaveChangesAsync();

            var result = await _loanService.GetAllLoanMastersAsync();

            Assert.NotNull(result);
            Assert.AreEqual(2, result.LoanMasters.Count);

            var firstLoanMaster = result.LoanMasters.First();
            Assert.AreEqual("HomeLoan", firstLoanMaster.LoanTypeName);
            Assert.AreEqual("CarLoan", result.LoanMasters.Last().LoanTypeName);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
