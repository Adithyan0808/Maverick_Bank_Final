

using AutoMapper;
using MaverickBank.Contexts;
using MaverickBank.Interfaces;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MaverickBank.Services
{
    public class BranchService : IBranchService
    {
        private readonly MaverickBankContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BranchService> _logger;

        public BranchService(MaverickBankContext context, IMapper mapper, ILogger<BranchService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BranchDropdownDto> GetAllBranchesAsync()
        {
            _logger.LogInformation("Fetching all branches");

            var branches = await _context.Branches.ToListAsync();
            var branchDtos = _mapper.Map<List<BranchDto>>(branches);

            _logger.LogInformation("Fetched {BranchCount} branches", branchDtos.Count);

            return new BranchDropdownDto { Branches = branchDtos };
        }

        public async Task<List<BranchDetailsDto>> GetAllBranchDetailsAsync()
        {
            _logger.LogInformation("Fetching detailed branch information");

            var branches = await _context.Branches.ToListAsync();
            var branchDetails = branches.Select(b => new BranchDetailsDto
            {
                BranchId = b.BranchId,
                BranchName = b.BranchName,
                Location = b.Location
            }).ToList();

            _logger.LogInformation("Fetched {BranchCount} branch details", branchDetails.Count);

            return branchDetails;
        }
    }
}
