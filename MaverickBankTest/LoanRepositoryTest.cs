using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MaverickBankTest
{
    [TestFixture]
    public class LoanMasterRepositoryTests
    {
        private DbContextOptions<MaverickBankContext> _options;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new MaverickBankContext(_options);
            context.LoanMasters.Add(new LoanMaster
            {
                LoanMasterId = 1,
                LoanTypeName = "TestLoanType",
                MaxLoanAmount = 100000,
                DefaultInterestRate = 7.5f
            });
            context.SaveChanges();
        }

        [Test]
        public async Task CreateLoanMaster_ShouldAddLoanMaster()
        {
            using var context = new MaverickBankContext(_options);

            var loanMaster = new LoanMaster
            {
                LoanTypeName = "TestType2",
                MaxLoanAmount = 250000,
                DefaultInterestRate = 9.2f
            };

            context.LoanMasters.Add(loanMaster);
            await context.SaveChangesAsync();

            var result = await context.LoanMasters.FirstOrDefaultAsync(lm => lm.LoanTypeName == "TestType2");

            Assert.IsNotNull(result);
            Assert.AreEqual(250000, result.MaxLoanAmount);
            Assert.AreEqual(9.2f, result.DefaultInterestRate);
        }

        [Test]
        public async Task GetLoanMasterById_ShouldReturnCorrectLoanMaster()
        {
            using var context = new MaverickBankContext(_options);

            var result = await context.LoanMasters.FindAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("TestLoanType", result.LoanTypeName);
        }

        [Test]
        public async Task UpdateLoanMaster_ShouldModifyLoanMaster()
        {
            using var context = new MaverickBankContext(_options);
            var loanMaster = await context.LoanMasters.FindAsync(1);

            loanMaster.MaxLoanAmount = 300000;
            context.LoanMasters.Update(loanMaster);
            await context.SaveChangesAsync();

            var updated = await context.LoanMasters.FindAsync(1);

            Assert.AreEqual(300000, updated.MaxLoanAmount);
        }

        [Test]
        public async Task DeleteLoanMaster_ShouldRemoveLoanMaster()
        {
            using var context = new MaverickBankContext(_options);
            var loanMaster = await context.LoanMasters.FindAsync(1);

            context.LoanMasters.Remove(loanMaster);
            await context.SaveChangesAsync();

            var deleted = await context.LoanMasters.FindAsync(1);

            Assert.IsNull(deleted);
        }


        [Test]
        public async Task ApplyLoanAsync_ShouldThrowException_WhenLoanAlreadyExists()
        {
            using var context = new MaverickBankContext(_options);
            var loanRepository = new LoanRepository(context);

            var loan = new Loan
            {
                LoanId = 1, // Explicitly setting same ID
                CustomerId = 1,
                LoanMasterId = 1,
                LoanAmount = 50000,
                LoanStatus = LoanStatus.Pending
            };

            await loanRepository.ApplyLoanAsync(loan);

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await loanRepository.ApplyLoanAsync(loan));

            StringAssert.Contains("An item with the same key has already been added", exception.Message);
        }



        [Test]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoLoansExist()
        {
            using var context = new MaverickBankContext(_options);
            var loanRepository = new LoanRepository(context);

            var result = await loanRepository.GetAll();

            Assert.IsEmpty(result);
        }



        [Test]
        public async Task GetLoanMasterByIdAsync_ShouldReturnLoanMaster_WhenExists()
        {
            using var context = new MaverickBankContext(_options);
            var loanRepository = new LoanRepository(context);

            var result = await loanRepository.GetLoanMasterByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("TestLoanType", result.LoanTypeName);
        }

        [Test]
        public async Task GetLoansByCustomerIdAsync_ShouldReturnLoansForCorrectCustomer()
        {
            using var context = new MaverickBankContext(_options);
            var loanRepository = new LoanRepository(context);

            var loan1 = new Loan
            {
                CustomerId = 1,
                LoanMasterId = 1,
                LoanAmount = 50000,
                LoanStatus = LoanStatus.Approved
            };

            var loan2 = new Loan
            {
                CustomerId = 2,
                LoanMasterId = 1,
                LoanAmount = 100000,
                LoanStatus = LoanStatus.Pending
            };

            context.Loans.Add(loan1);
            context.Loans.Add(loan2);
            await context.SaveChangesAsync();

            var result = await loanRepository.GetLoansByCustomerIdAsync(1);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().CustomerId);
        }





    }
}
