using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using MaverickBank.Repositories;
using MaverickBank.Services;
using Microsoft.Extensions.Logging;


namespace MaverickBankTest.ServiceTest
{
    [TestFixture]
    public class TransactionServiceTest
    {
        private MaverickBankContext _context;
        private TransactionRepository _transactionRepo;
        private AccountRepository _accountRepo;
        private IMapper _mapper;
        private TransactionService _transactionService;
        private ILogger<TransactionService> _logger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: "TestTransactionDb")
                .Options;

            _context = new MaverickBankContext(options);

            _context.TransactionTypes.AddRange(
                new TransactionType { TransactionTypeId = 1, TransactionTypeName = "Withdraw" },
                new TransactionType { TransactionTypeId = 2, TransactionTypeName = "Deposit" },
                new TransactionType { TransactionTypeId = 3, TransactionTypeName = "Transfer" },
                new TransactionType { TransactionTypeId = 4, TransactionTypeName = "Loan Repayment" }
            );

            _context.Customers.Add(new Customer
            {
                CustomerId = 1,
                FullName = "John Doe",
                AadharNumber = "123456789012",
                Address = "123 Main St",
                Email = "john@example.com",
                Gender = "Male",
                PANNumber = "ABCDE1234F",
                PhoneNumber = "9876543210"
            });

            _context.Accounts.AddRange(
                new Account { AccountId = 1, AccountNumber = "ACC001", Balance = 1000, CustomerId = 1 },
                new Account { AccountId = 2, AccountNumber = "ACC002", Balance = 500, CustomerId = 1 }
            );

            _context.SaveChanges();

            _transactionRepo = new TransactionRepository(_context);
            _accountRepo = new AccountRepository(_context);
            _mapper = CreateMapper();
         
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<TransactionService>();

            _transactionService = new TransactionService(_transactionRepo, _accountRepo, _mapper, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Transaction, TransactionResponseDTO>();
            });
            return config.CreateMapper();
        }

        [Test]
        public async Task MakeTransaction_Deposit_ShouldIncreaseBalance()
        {
            _logger.LogInformation("Starting test for Deposit transaction...");

            var request = new TransactionRequestDTO
            {
                SourceAccountNumber = "ACC001",
                Amount = 500,
                TransactionTypeId = 2, // Deposit
                CustomerId = 1
            };

            var result = await _transactionService.MakeTransaction(request);
            var updatedAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == "ACC001");

            Assert.IsNotNull(result);
            Assert.AreEqual(1500, updatedAccount.Balance);

            _logger.LogInformation("Deposit transaction completed with updated balance: {Balance}", updatedAccount.Balance);
        }

        [Test]
        public async Task MakeTransaction_Withdraw_ShouldDecreaseBalance()
        {
            _logger.LogInformation("Starting test for Withdrawal transaction...");

            var request = new TransactionRequestDTO
            {
                SourceAccountNumber = "ACC001",
                Amount = 400,
                TransactionTypeId = 1, // Withdraw
                CustomerId = 1
            };

            var result = await _transactionService.MakeTransaction(request);
            var updatedAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == "ACC001");

            Assert.IsNotNull(result);
            Assert.AreEqual(600, updatedAccount.Balance);

            _logger.LogInformation("Withdrawal transaction completed with updated balance: {Balance}", updatedAccount.Balance);
        }

        [Test]
        public async Task MakeTransaction_Transfer_ShouldMoveBalance()
        {
            _logger.LogInformation("Starting test for Transfer transaction...");

            var request = new TransactionRequestDTO
            {
                SourceAccountNumber = "ACC001",
                DestinationAccountNumber = "ACC002",
                Amount = 300,
                TransactionTypeId = 3, // Transfer
                CustomerId = 1
            };

            var result = await _transactionService.MakeTransaction(request);

            var source = await _context.Accounts.FirstAsync(a => a.AccountNumber == "ACC001");
            var destination = await _context.Accounts.FirstAsync(a => a.AccountNumber == "ACC002");

            Assert.IsNotNull(result);
            Assert.AreEqual(700, source.Balance);
            Assert.AreEqual(800, destination.Balance);

            _logger.LogInformation("Transfer transaction completed from source account {SourceAccount} to destination account {DestinationAccount}", source.AccountNumber, destination.AccountNumber);
        }

        [Test]
        public async Task GetAllTransactions_ShouldReturnList()
        {
            _logger.LogInformation("Starting test for fetching all transactions...");

            await MakeTransaction_Deposit_ShouldIncreaseBalance();
            var result = await _transactionService.GetAllTransactions();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());

            _logger.LogInformation("Fetched all transactions, count: {Count}", result.Count());
        }

        [Test]
        public async Task GetTransactionsByCustomerId_ShouldReturnCorrectCount()
        {
            _logger.LogInformation("Starting test for fetching transactions by customer ID...");

            await MakeTransaction_Deposit_ShouldIncreaseBalance();
            var result = await _transactionService.GetTransactionsByCustomerId(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());

            _logger.LogInformation("Fetched transactions for customer ID 1, count: {Count}", result.Count());
        }

        [Test]
        public async Task GetRecentTransactionsByCustomerId_ShouldReturnLast3()
        {
            _logger.LogInformation("Starting test for fetching recent transactions by customer ID...");

            for (int i = 0; i < 5; i++)
            {
                await _transactionService.MakeTransaction(new TransactionRequestDTO
                {
                    SourceAccountNumber = "ACC001",
                    Amount = 10,
                    TransactionTypeId = 2,
                    CustomerId = 1
                });
            }

            var result = await _transactionService.GetRecentTransactionsByCustomerId(1);
            Assert.AreEqual(3, result.Count());

            _logger.LogInformation("Fetched recent transactions for customer ID 1, count: {Count}", result.Count());
        }
    }
}






