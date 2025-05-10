using AutoMapper;
using MaverickBank.Contexts;
using MaverickBank.Interfaces;
using MaverickBank.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MaverickBank.Services
{
    public class TransactionTypeService : ITransactionTypeService
    {
        private readonly MaverickBankContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionTypeService> _logger;

        public TransactionTypeService(MaverickBankContext context, IMapper mapper, ILogger<TransactionTypeService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TransactionTypeDropdownDto> GetAllTransactionTypesAsync()
        {
            _logger.LogInformation("Fetching all transaction types from the database.");

            var transactionTypes = await _context.TransactionTypes.ToListAsync();
            var transactionTypeDtos = _mapper.Map<List<TransactionTypeDto>>(transactionTypes);

            _logger.LogInformation("Mapped {Count} transaction types to DTOs.", transactionTypeDtos.Count);

            return new TransactionTypeDropdownDto { TransactionTypes = transactionTypeDtos };
        }
    }
}
