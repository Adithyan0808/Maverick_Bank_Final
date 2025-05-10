using AutoMapper;
using MaverickBank.Interfaces;
using MaverickBank.Models.DTOs;
using MaverickBank.Repositories;
using Microsoft.Extensions.Logging;

namespace MaverickBank.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerRepository _customerRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(CustomerRepository customerRepo, IMapper mapper, ILogger<CustomerService> logger)
        {
            _customerRepo = customerRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CustomerSelfDetailsDTO> GetCustomerDetailsAsync(int customerId)
        {
            _logger.LogInformation("Fetching customer details for CustomerId: {CustomerId}", customerId);
            var customer = await _customerRepo.GetCustomerWithDetailsAsync(customerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer with ID {CustomerId} not found.", customerId);
                return null;
            }

            _logger.LogInformation("Successfully fetched customer details for CustomerId: {CustomerId}", customerId);
            return _mapper.Map<CustomerSelfDetailsDTO>(customer);
        }

        public async Task<CustomerSelfDetailsDTO> UpdateCustomerAsync(int customerId, CustomerUpdateDTO dto)
        {
            _logger.LogInformation("Updating customer with ID: {CustomerId}", customerId);
            var updatedCustomer = await _customerRepo.UpdateCustomerAsync(customerId, dto);
            if (updatedCustomer == null)
            {
                _logger.LogWarning("Failed to update customer with ID: {CustomerId}", customerId);
                return null;
            }

            _logger.LogInformation("Successfully updated customer with ID: {CustomerId}", customerId);
            return _mapper.Map<CustomerSelfDetailsDTO>(updatedCustomer);
        }

        public async Task<IEnumerable<AccountDetailsDTO>> GetAccountsByCustomerIdAsync(int customerId)
        {
            _logger.LogInformation("Fetching accounts for CustomerId: {CustomerId}", customerId);
            var accounts = await _customerRepo.GetAccountsByCustomerIdAsync(customerId);

            if (accounts == null || !accounts.Any())
            {
                _logger.LogWarning("No accounts found for CustomerId: {CustomerId}", customerId);
                return Enumerable.Empty<AccountDetailsDTO>();
            }

            _logger.LogInformation("Successfully fetched {Count} accounts for CustomerId: {CustomerId}", accounts.Count(), customerId);
            return _mapper.Map<IEnumerable<AccountDetailsDTO>>(accounts);
        }
    }
}
