using MaverickBank.Contexts;
using MaverickBank.Models;
using Microsoft.EntityFrameworkCore;

namespace MaverickBank.Repositories
{
    public class LoanRepository : Repository<int, Loan>
    {
        private readonly MaverickBankContext _context;

        public LoanRepository(MaverickBankContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Loan> ApplyLoanAsync(Loan loan)
        {
            loan.CreatedAt = DateTime.UtcNow;
            loan.LoanStatus = LoanStatus.Pending;
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return await _context.Loans
                .Include(l => l.LoanMaster)
                .FirstOrDefaultAsync(l => l.LoanId == loan.LoanId);
        }

        public async Task<LoanMaster?> GetLoanMasterByIdAsync(int loanMasterId)
        {
            return await _context.LoanMasters
                .FirstOrDefaultAsync(lm => lm.LoanMasterId == loanMasterId);
        }

        public async Task<IEnumerable<Loan>> GetLoansByCustomerIdAsync(int customerId)
        {
            return await _context.Loans
                .Include(l => l.LoanMaster)
                .Where(l => l.CustomerId == customerId)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Loan>> GetAll()
        {
            return await _context.Loans
                .Include(l => l.LoanMaster)
                .ToListAsync();
        }

        public override async Task<Loan?> GetById(int id)
        {
            return await _context.Loans
                .Include(l => l.LoanMaster)
                .FirstOrDefaultAsync(l => l.LoanId == id);
        }

        //now
        public async Task<Loan> UpdateLoanStatusAsync(int loanId, LoanStatus newStatus)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null)
            {
                throw new KeyNotFoundException($"Loan with ID {loanId} not found.");
            }

            loan.LoanStatus = newStatus;
            await _context.SaveChangesAsync();

            return await _context.Loans
                .Include(l => l.LoanMaster)
                .FirstOrDefaultAsync(l => l.LoanId == loanId);
        }

        public async Task<List<LoanMaster>> GetAllLoanMastersAsync()
        {
            return await _context.LoanMasters.ToListAsync();
        }


    }
}
