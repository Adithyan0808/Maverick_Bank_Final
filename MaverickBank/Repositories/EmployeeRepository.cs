using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaverickBank.Repositories
{
    public class EmployeeRepository : Repository<int, Employee>
    {
        private readonly MaverickBankContext _context;

        public EmployeeRepository(MaverickBankContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<Employee>> GetAll()
        {
            var employees = await _context.Employees.Include(e => e.Branch).ToListAsync();
            if (!employees.Any())
                throw new Exception("No employees found");
            return employees;
        }

        public override async Task<Employee> GetById(int id)
        {
            var employee = await _context.Employees
                                         .Include(e => e.Branch)
                                         .FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (employee == null)
                throw new Exception("Employee not found");
            return employee;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers
                .Include(c => c.Accounts)
                    .ThenInclude(a => a.AccountType)
                .ToListAsync();

            if (!customers.Any())
                throw new Exception("No customers found");

            return customers;
        }

        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Accounts)
                    .ThenInclude(a => a.AccountType) 
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                throw new Exception("Customer not found");

            return customer;
        }

        
        public async Task<IEnumerable<Customer>> GetCustomersByAccountTypeAsync(string accountTypeName)
        {
            var customers = await _context.Customers
                .Include(c => c.Accounts)
                    .ThenInclude(a => a.AccountType)
                .Where(c => c.Accounts.Any(a => a.AccountType.AccountTypeName == accountTypeName))
                .ToListAsync();

            if (!customers.Any())
                throw new Exception("No customers found for the given account type");

            return customers;
        }

       
        public async Task<Customer> UpdateCustomerAsync(int customerId, CustomerUpdateDTO updateDto)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                throw new Exception("Customer not found");

            // Update fields if provided
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

            var updatedCustomer = await _context.Customers
                .Include(c => c.Accounts)
                    .ThenInclude(a => a.AccountType)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            return updatedCustomer!;
        }

        public async Task<object> DeleteCustomerByIdAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                throw new Exception("Customer not found");

            string name = customer.FullName;

            _context.Customers.Remove(customer);

            // Also remove associated user if exists
            if (customer.User != null)
                _context.Users.Remove(customer.User);

            await _context.SaveChangesAsync();

            return new
            {
                customerId = customerId,
                name = name,
                message = "deleted successfully"
            };
        }

    }
}
