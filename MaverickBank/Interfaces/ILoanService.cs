// Interfaces/ILoanService.cs
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaverickBank.Interfaces
{
    public interface ILoanService
    {
        // Method to apply for a loan
        Task<LoanApplicationResponseDTO> ApplyForLoanAsync(LoanApplicationDTO loanApplicationDTO);

        // Method to get loans by customer ID
        Task<IEnumerable<LoanApplicationResponseDTO>> GetLoansByCustomerIdAsync(int customerId);
        
        Task<IEnumerable<LoanApplicationResponseDTO>> GetAllLoansAsync();
        //now
        Task<LoanApplicationResponseDTO> UpdateLoanStatusAsync(int loanId, LoanStatus newStatus);

        Task<LoanDropdownDto> GetAllLoanMastersAsync();

    }
}
