using MaverickBank.Models.DTOs;

namespace MaverickBank.Interfaces
{
    public interface IAdminRegistrationService
    {
        Task<RegisterResponseDTO> RegisterAdminAsync(RegisterAdminDTO dto);
    }
}
