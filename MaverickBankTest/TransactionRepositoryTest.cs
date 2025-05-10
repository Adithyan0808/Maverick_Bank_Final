using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;


namespace MaverickBankTest
{
    [TestFixture]
    public class TransactionRepositoryTests
    {
        private DbContextOptions<MaverickBankContext> _options;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new MaverickBankContext(_options);
            context.TransactionTypes.Add(new TransactionType
            {
                TransactionTypeId = 1,
                TransactionTypeName = "Deposit"
            });

            context.TransactionTypes.Add(new TransactionType
            {
                TransactionTypeId = 2,
                TransactionTypeName = "Withdrawal"
            });

            context.Accounts.Add(new Account
            {
                AccountId = 1,
                AccountNumber = "ACC123",
                Balance = 1000
            });

            context.Accounts.Add(new Account
            {
                AccountId = 2,
                AccountNumber = "ACC124",
                Balance = 2000
            });

            context.Customers.Add(new Customer
            {
                CustomerId = 1,
                FullName = "Test Customer 1",
                Email = "test1@example.com",
                AadharNumber = "1234-5678-9876",
                Address = "Test Address, City, Country",
                Gender = "Male", 
                PANNumber = "ABCDE1234F",
                PhoneNumber = "9876543210"
            });

            context.Transactions.Add(new Transaction
            {
                TransactionId = 1,
                Amount = 200,
                TransactionTypeId = 1,  // Deposit
                SourceAccountId = 1,
                DestinationAccountId = 2,
                CustomerId = 1,
                TransactionDate = DateTime.Now.AddDays(-1)
            });

            context.Transactions.Add(new Transaction
            {
                TransactionId = 2,
                Amount = 150,
                TransactionTypeId = 2,  // Withdrawal
                SourceAccountId = 2,
                DestinationAccountId = 1,
                CustomerId = 1,
                TransactionDate = DateTime.Now
            });

            context.SaveChanges();
        }

        [Test]
        public async Task GetAllTransactions_ShouldReturnTransactions()
        {
            using var context = new MaverickBankContext(_options);
            var repo = new TransactionRepository(context);

            var transactions = await repo.GetAll();

            Assert.IsNotNull(transactions);
            Assert.AreEqual(2, transactions.Count());
        }

        [Test]
        public async Task GetTransactionById_ShouldReturnCorrectTransaction()
        {
            using var context = new MaverickBankContext(_options);
            var repo = new TransactionRepository(context);

            var transaction = await repo.GetById(1);

            Assert.IsNotNull(transaction);
            Assert.AreEqual(200, transaction.Amount);
            Assert.AreEqual("Deposit", transaction.TransactionType.TransactionTypeName);
        }

        [Test]
        public async Task GetTransactionsByCustomerId_ShouldReturnTransactions()
        {
            using var context = new MaverickBankContext(_options);
            var repo = new TransactionRepository(context);

            var transactions = await repo.GetTransactionsByCustomerIdAsync(1);

            Assert.IsNotNull(transactions);
            Assert.AreEqual(2, transactions.Count());
        }

        [Test]
        public async Task GetTransactionsByCustomerWithFilters_ShouldReturnFilteredTransactions()
        {
            using var context = new MaverickBankContext(_options);
            var repo = new TransactionRepository(context);

            var transactions = await repo.GetTransactionsByCustomerWithFiltersAsync(1, transactionTypeId: 1);

            Assert.IsNotNull(transactions);
            Assert.AreEqual(1, transactions.Count());
            Assert.AreEqual("Deposit", transactions.First().TransactionType.TransactionTypeName);
        }

        [Test]
        public async Task GetTransactionTypeById_ShouldReturnTransactionType()
        {
            using var context = new MaverickBankContext(_options);
            var repo = new TransactionRepository(context);

            var transactionType = await repo.GetTransactionTypeByIdAsync(1);

            Assert.IsNotNull(transactionType);
            Assert.AreEqual("Deposit", transactionType.TransactionTypeName);
        }

        [Test]
        public async Task GetAccountByNumber_ShouldReturnCorrectAccount()
        {
            using var context = new MaverickBankContext(_options);
            var repo = new TransactionRepository(context);

            var account = await repo.GetAccountByNumberAsync("ACC123");

            Assert.IsNotNull(account);
            Assert.AreEqual("ACC123", account.AccountNumber);
        }

        [Test]
        public async Task GetTransactionsByCustomerWithDateRange_ShouldReturnFilteredTransactions()
        {
            using var context = new MaverickBankContext(_options);
            var repo = new TransactionRepository(context);

            var fromDate = DateTime.Now.AddDays(-2);
            var toDate = DateTime.Now;

            var transactions = await repo.GetTransactionsByCustomerWithFiltersAsync(1, fromDate: fromDate, toDate: toDate);

            Assert.IsNotNull(transactions);
            Assert.AreEqual(2, transactions.Count());
        }




    }
}
