using MaverickBank.Models;

namespace MaverickBank.Interfaces
{
    public interface ITransactionRepository : IRepository<int, Transaction>
    {
        Task<Account> GetAccountByNumberAsync(string accountNumber);
        Task<TransactionType> GetTransactionTypeByIdAsync(int transactionTypeId);
        Task<IEnumerable<Transaction>> GetTransactionsByCustomerIdAsync(int customerId);
        //now
        Task<IEnumerable<Transaction>> GetTransactionsByCustomerWithFiltersAsync(
    int customerId, int? transactionTypeId = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
