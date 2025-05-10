using AutoMapper;
using MaverickBank.Interfaces;
using MaverickBank.Models.DTOs;
using MaverickBank.Repositories;
using Microsoft.Extensions.Logging;

namespace MaverickBank.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IRepository<int, Employee> employeeRepository, IMapper mapper, ILogger<EmployeeService> logger)
        {
            _employeeRepository = (EmployeeRepository)employeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomerDetailsDTO>> GetAllCustomersAsync()
        {
            _logger.LogInformation("Fetching all customers.");
            var customers = await _employeeRepository.GetAllCustomersAsync();
            _logger.LogInformation($"Fetched {customers.Count()} customers.");
            return _mapper.Map<IEnumerable<CustomerDetailsDTO>>(customers);
        }

        public async Task<CustomerDetailsDTO> GetCustomerByIdAsync(int customerId)
        {
            _logger.LogInformation($"Fetching customer with ID: {customerId}");
            var customer = await _employeeRepository.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                _logger.LogWarning($"Customer with ID: {customerId} not found.");
            }
            else
            {
                _logger.LogInformation($"Fetched customer with ID: {customerId}.");
            }
            return _mapper.Map<CustomerDetailsDTO>(customer);
        }

        public async Task<IEnumerable<CustomerDetailsDTO>> GetCustomersByAccountTypeAsync(string accountTypeName)
        {
            _logger.LogInformation($"Fetching customers with account type: {accountTypeName}");
            var customers = await _employeeRepository.GetCustomersByAccountTypeAsync(accountTypeName);
            _logger.LogInformation($"Fetched {customers.Count()} customers with account type: {accountTypeName}.");
            return _mapper.Map<IEnumerable<CustomerDetailsDTO>>(customers);
        }

        public async Task<CustomerDetailsDTO> UpdateCustomerAsync(int customerId, CustomerUpdateDTO updateDto)
        {
            _logger.LogInformation($"Updating customer with ID: {customerId}");
            var updatedCustomer = await _employeeRepository.UpdateCustomerAsync(customerId, updateDto);
            _logger.LogInformation($"Updated customer with ID: {customerId}.");
            return _mapper.Map<CustomerDetailsDTO>(updatedCustomer);
        }

        public async Task<object> DeleteCustomerAsync(int customerId)
        {
            _logger.LogInformation($"Deleting customer with ID: {customerId}");
            var result = await _employeeRepository.DeleteCustomerByIdAsync(customerId);
            if (result != null)
            {
                _logger.LogInformation($"Successfully deleted customer with ID: {customerId}");
            }
            else
            {
                _logger.LogWarning($"Failed to delete customer with ID: {customerId}.");
            }
            return result;
        }
    }
}
