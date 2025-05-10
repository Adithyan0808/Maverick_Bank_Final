using MaverickBank.Contexts;
using MaverickBank.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaverickBank.Repositories
{
    public class AccountRepository : Repository<int, Account>
    {
        public AccountRepository(MaverickBankContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Account>> GetAll()
        {
            var accounts = await _context.Accounts.Include(a => a.Customer).ToListAsync();
            if (!accounts.Any())
                throw new Exception("No accounts found");
            return accounts;
        }

        public override async Task<Account> GetById(int id)
        {
            var account = await _context.Accounts.Include(a => a.Customer)
                                                 .FirstOrDefaultAsync(a => a.AccountId == id);
            if (account == null)
                throw new Exception("Account not found");
            return account;
        }

        public async Task<Account> GetByAccountNumberAsync(string accountNumber)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            if (account == null)
                throw new Exception("Account not found with number: " + accountNumber);
            return account;
        }
    }
}
