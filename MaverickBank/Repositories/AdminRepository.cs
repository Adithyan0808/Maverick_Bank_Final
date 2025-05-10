

using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaverickBank.Repositories
{
    public class AdminRepository : Repository<int, Admin>
    {
        private readonly MaverickBankContext _context;

        public AdminRepository(MaverickBankContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<Admin>> GetAll()
        {
            var admins = await _context.Admins.ToListAsync();
            return admins;
        }

        public override async Task<Admin> GetById(int id)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.AdminId == id);
            return admin;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees
                .Include(e => e.Branch)
                .ToListAsync();

            if (!employees.Any())
                throw new Exception("No employees found");

            return employees;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.Branch)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
                throw new Exception("Employee not found");

            return employee;
        }
        
        public async Task<Employee> UpdateEmployeeDetailsAsync(int employeeId, EmployeeUpdateDTO updateDto)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
                throw new Exception("Employee not found");

            // Update fields if provided
            if (!string.IsNullOrWhiteSpace(updateDto.FullName))
                employee.FullName = updateDto.FullName;
            if (!string.IsNullOrWhiteSpace(updateDto.PhoneNumber))
                employee.PhoneNumber = updateDto.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(updateDto.Email))
                employee.Email = updateDto.Email;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return employee;
        }

        //now
        public async Task<object> DeleteEmployeeByIdAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
                throw new Exception("Employee not found");

            string name = employee.FullName;

            // Delete associated user if exists
            if (employee.User != null)
                _context.Users.Remove(employee.User);

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return new
            {
                employeeId = employeeId,
                name = name,
                message = "deleted successfully"
            };
        }




    }
}
