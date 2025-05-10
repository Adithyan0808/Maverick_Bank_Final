using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MaverickBank.Repositories
{
    public class CustomerRepository : Repository<int, Customer>
    {
        private readonly MaverickBankContext _context;

        public CustomerRepository(MaverickBankContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<Customer>> GetAll()
        {
            return await _context.Customers.ToListAsync();
        }

        public override async Task<Customer> GetById(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task<Customer> GetCustomerWithDetailsAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Accounts)
                    .ThenInclude(a => a.AccountType)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                throw new Exception("Customer not found");

            return customer;
        }
       
        public async Task<Customer> UpdateCustomerAsync(int customerId, CustomerUpdateDTO updateDto)
        {
            var customer = await _context.Customers
                .Include(c => c.Accounts)
                    .ThenInclude(a => a.AccountType)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                throw new Exception("Customer not found");

            // Update provided fields
            if (!string.IsNullOrWhiteSpace(updateDto.FullName))
                customer.FullName = updateDto.FullName;
            if (!string.IsNullOrWhiteSpace(updateDto.Gender))
                customer.Gender = updateDto.Gender;
            if (!string.IsNullOrWhiteSpace(updateDto.PhoneNumber))
                customer.PhoneNumber = updateDto.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(updateDto.Email))
                customer.Email = updateDto.Email;
            if (!string.IsNullOrWhiteSpace(updateDto.Address))
                customer.Address = updateDto.Address;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        
        public async Task<IEnumerable<Account>> GetAccountsByCustomerIdAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Accounts)
                    .ThenInclude(a => a.AccountType)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                throw new Exception("Customer not found");

            return customer.Accounts;
        }

        





    }

}
