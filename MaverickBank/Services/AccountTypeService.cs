
using AutoMapper;
using MaverickBank.Contexts;
using MaverickBank.Interfaces;
using MaverickBank.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MaverickBank.Services
{
    public class AccountTypeService : IAccountTypeService
    {
        private readonly MaverickBankContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountTypeService> _logger;

        public AccountTypeService(
            MaverickBankContext context,
            IMapper mapper,
            ILogger<AccountTypeService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AccountTypeDropdownDto> GetAllAccountTypesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all account types from the database.");
                var accountTypes = await _context.AccountTypes.ToListAsync();

                _logger.LogInformation("Mapping account types to DTOs.");
                var accountTypeDtos = _mapper.Map<List<AccountTypeDto>>(accountTypes);

                _logger.LogInformation("Returning AccountTypeDropdownDto with {Count} account types.", accountTypeDtos.Count);
                return new AccountTypeDropdownDto { AccountTypes = accountTypeDtos };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving account types.");
                throw;
            }
        }
    }
}






