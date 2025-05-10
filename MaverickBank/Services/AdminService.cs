using AutoMapper;
using MaverickBank.Interfaces;
using MaverickBank.Models.DTOs;
using MaverickBank.Repositories;
using Microsoft.Extensions.Logging;

namespace MaverickBank.Services
{
    public class AdminService : IAdminService
    {
        private readonly AdminRepository _adminRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;

        public AdminService(AdminRepository adminRepo, IMapper mapper, ILogger<AdminService> logger)
        {
            _adminRepo = adminRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<EmployeeDetailsDTO>> GetAllEmployeesAsync()
        {
            _logger.LogInformation("Fetching all employees.");

            var employees = await _adminRepo.GetAllEmployeesAsync();
            var result = _mapper.Map<IEnumerable<EmployeeDetailsDTO>>(employees);

            _logger.LogInformation("Fetched {Count} employees.", result.Count());
            return result;
        }

        public async Task<EmployeeDetailsDTO> GetEmployeeByIdAsync(int id)
        {
            _logger.LogInformation("Fetching employee with ID: {Id}", id);

            var employee = await _adminRepo.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {Id} not found.", id);
                return null;
            }

            var result = _mapper.Map<EmployeeDetailsDTO>(employee);
            _logger.LogInformation("Fetched employee: {EmployeeName}", result.FullName);
            return result;
        }

        public async Task<EmployeeDTO> UpdateEmployeeDetailsAsync(int employeeId, EmployeeUpdateDTO updateDto)
        {
            _logger.LogInformation("Updating employee with ID: {Id}", employeeId);

            var updatedEmployee = await _adminRepo.UpdateEmployeeDetailsAsync(employeeId, updateDto);
            var result = _mapper.Map<EmployeeDTO>(updatedEmployee);

            _logger.LogInformation("Updated employee {Id} successfully.", employeeId);
            return result;
        }

        public async Task<object> DeleteEmployeeByIdAsync(int employeeId)
        {
            _logger.LogInformation("Deleting employee with ID: {Id}", employeeId);

            var result = await _adminRepo.DeleteEmployeeByIdAsync(employeeId);

            if (result == null)
            {
                _logger.LogWarning("Attempted to delete non-existing employee with ID: {Id}", employeeId);
            }
            else
            {
                _logger.LogInformation("Deleted employee with ID: {Id}", employeeId);
            }

            return result;
        }
    }
}
