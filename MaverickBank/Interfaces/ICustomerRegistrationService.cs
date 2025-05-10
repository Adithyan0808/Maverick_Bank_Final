using MaverickBank.Models.DTOs;

namespace MaverickBank.Interfaces
{
    public interface ICustomerRegistrationService
    {
        Task<RegisterResponseDTO> RegisterCustomerAsync(RegisterCustomerDTO dto);

    }
}
