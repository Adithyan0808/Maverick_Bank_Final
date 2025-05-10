using MaverickBank.Models.DTOs;

namespace MaverickBank.Interfaces
{
    public interface IAccountTypeService
    {
        Task<AccountTypeDropdownDto> GetAllAccountTypesAsync();
    }
}
