using MaverickBank.Models.DTOs;

namespace MaverickBank.Interfaces
{
    public interface IEmployeeRegistrationService
    {
        Task<RegisterResponseDTO> RegisterEmployeeAsync(RegisterEmployeeDTO dto);
    }
}
