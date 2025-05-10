using MaverickBank.Contexts;
using MaverickBank.Interfaces;
using MaverickBank.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaverickBank.Repositories
{
    public class TransactionRepository : Repository<int, Transaction>, ITransactionRepository
    {
        public TransactionRepository(MaverickBankContext context) : base(context) { }

        public override async Task<IEnumerable<Transaction>> GetAll()
        {
            return await _context.Transactions
                .Include(t => t.TransactionType)
                .Include(t => t.SourceAccount)
                .Include(t => t.DestinationAccount)
                .ToListAsync();
        }

        public override async Task<Transaction> GetById(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.TransactionType)
                .Include(t => t.SourceAccount)
                .Include(t => t.DestinationAccount)
                .FirstOrDefaultAsync(t => t.TransactionId == id);

            if (transaction == null)
                throw new KeyNotFoundException("Transaction not found");

            return transaction;
        }

        public async Task<Account> GetAccountByNumberAsync(string accountNumber)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            if (account == null)
                throw new KeyNotFoundException("Account not found");

            return account;
        }

        public async Task<TransactionType> GetTransactionTypeByIdAsync(int transactionTypeId)
        {
            var transactionType = await _context.TransactionTypes.FindAsync(transactionTypeId);
            if (transactionType == null)
                throw new KeyNotFoundException("Transaction type not found");

            return transactionType;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByCustomerIdAsync(int customerId)
        {
            var transactions = await _context.Transactions
                .Include(t => t.TransactionType)
                .Where(t => t.CustomerId == customerId)
                .ToListAsync();

            if (!transactions.Any())
                throw new Exception("No transactions found for the given customer ID");

            return transactions;
        }
        public async Task<IEnumerable<Transaction>> GetTransactionsByCustomerWithFiltersAsync(
    int customerId, int? transactionTypeId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Transactions
                .Include(t => t.TransactionType)
                .Where(t => t.CustomerId == customerId)
                .AsQueryable();

            if (transactionTypeId.HasValue)
                query = query.Where(t => t.TransactionTypeId == transactionTypeId.Value);

            if (fromDate.HasValue)
                query = query.Where(t => t.TransactionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.TransactionDate <= toDate.Value);

            var transactions = await query.ToListAsync();

            if (!transactions.Any())
                throw new Exception("No transactions found for the given criteria");

            return transactions;
        }


    }
}
