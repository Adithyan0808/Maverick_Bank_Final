using MaverickBank.Models.DTOs;

namespace MaverickBank.Interfaces
{
    public interface IBranchService
    {
        Task<BranchDropdownDto> GetAllBranchesAsync();
        Task<List<BranchDetailsDto>> GetAllBranchDetailsAsync();
    }
}
