using AutoMapper;
using MaverickBank.Interfaces;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using MaverickBank.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaverickBank.Services
{
    public class LoanService : ILoanService
    {
        private readonly LoanRepository _loanRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanService> _logger;

        public LoanService(LoanRepository loanRepository, IMapper mapper, ILogger<LoanService> logger)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<LoanApplicationResponseDTO> ApplyForLoanAsync(LoanApplicationDTO loanApplicationDTO)
        {
            _logger.LogInformation("Started processing loan application for customer ID: {CustomerId}", loanApplicationDTO.CustomerId);

            var loanMaster = await _loanRepository.GetLoanMasterByIdAsync(loanApplicationDTO.LoanMasterId);
            if (loanMaster == null)
            {
                _logger.LogWarning("Invalid loan type provided for customer ID: {CustomerId}", loanApplicationDTO.CustomerId);
                throw new ArgumentException("Invalid loan type.");
            }

            var loan = new Loan
            {
                CustomerId = loanApplicationDTO.CustomerId,
                LoanMasterId = loanMaster.LoanMasterId,
                LoanAmount = loanApplicationDTO.LoanAmount,
                LoanStatus = LoanStatus.Pending // Initial status
            };

            var appliedLoan = await _loanRepository.ApplyLoanAsync(loan);

            _logger.LogInformation("Successfully applied loan for customer ID: {CustomerId} with loan ID: {LoanId}", loanApplicationDTO.CustomerId, appliedLoan.LoanId);

            return _mapper.Map<LoanApplicationResponseDTO>(appliedLoan);
        }

        public async Task<IEnumerable<LoanApplicationResponseDTO>> GetLoansByCustomerIdAsync(int customerId)
        {
            _logger.LogInformation("Fetching loans for customer ID: {CustomerId}", customerId);

            var loans = await _loanRepository.GetLoansByCustomerIdAsync(customerId);

            if (loans == null || !loans.Any())
            {
                _logger.LogWarning("No loans found for customer ID: {CustomerId}", customerId);
                throw new KeyNotFoundException("The customer has not applied for loans.");
            }

            _logger.LogInformation("Found {LoanCount} loans for customer ID: {CustomerId}", loans.Count(), customerId);

            return _mapper.Map<IEnumerable<LoanApplicationResponseDTO>>(loans);
        }

        public async Task<IEnumerable<LoanApplicationResponseDTO>> GetAllLoansAsync()
        {
            _logger.LogInformation("Fetching all loans.");

            var loans = await _loanRepository.GetAll();

            if (!loans.Any())
            {
                _logger.LogWarning("No loans found in the system.");
                throw new KeyNotFoundException("No loans found.");
            }

            _logger.LogInformation("Found {LoanCount} loans in the system", loans.Count());

            return _mapper.Map<IEnumerable<LoanApplicationResponseDTO>>(loans);
        }

        public async Task<LoanApplicationResponseDTO> UpdateLoanStatusAsync(int loanId, LoanStatus newStatus)
        {
            _logger.LogInformation("Updating loan status for loan ID: {LoanId} to status: {NewStatus}", loanId, newStatus);

            var updatedLoan = await _loanRepository.UpdateLoanStatusAsync(loanId, newStatus);

            _logger.LogInformation("Successfully updated loan status for loan ID: {LoanId} to status: {NewStatus}", loanId, newStatus);

            return _mapper.Map<LoanApplicationResponseDTO>(updatedLoan);
        }

        public async Task<LoanDropdownDto> GetAllLoanMastersAsync()
        {
            _logger.LogInformation("Fetching all loan masters.");

            var loanMasters = await _loanRepository.GetAllLoanMastersAsync();
            var dtoList = _mapper.Map<List<LoanMasterDto>>(loanMasters);

            _logger.LogInformation("Found {LoanMasterCount} loan masters.", loanMasters.Count);

            return new LoanDropdownDto { LoanMasters = dtoList };
        }
    }
}
