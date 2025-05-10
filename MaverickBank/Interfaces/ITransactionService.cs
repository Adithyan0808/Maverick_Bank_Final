using MaverickBank.Models;
using System.Threading.Tasks;

namespace MaverickBank.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponseDTO> MakeTransaction(TransactionRequestDTO requestDto);
        Task<IEnumerable<TransactionResponseDTO>> GetAllTransactions();
        Task<TransactionResponseDTO> GetTransactionById(int id);
        Task<IEnumerable<TransactionResponseDTO>> GetTransactionsByCustomerId(int customerId);
        //now
        Task<IEnumerable<TransactionResponseDTO>> GetTransactionsByCustomerIdWithFilters(
    int customerId, int? transactionTypeId, DateTime? fromDate, DateTime? toDate);

        //04.05.25
        Task<IEnumerable<TransactionResponseDTO>> GetRecentTransactionsByCustomerId(int customerId);


    }
}

