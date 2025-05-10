using MaverickBank.Models.DTOs;

namespace MaverickBank.Interfaces
{
    public interface ITransactionTypeService
    {
        Task<TransactionTypeDropdownDto> GetAllTransactionTypesAsync();
    }
}
