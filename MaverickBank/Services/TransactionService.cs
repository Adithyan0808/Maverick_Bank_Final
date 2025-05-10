using AutoMapper;
using MaverickBank.Exceptions;
using MaverickBank.Interfaces;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaverickBank.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<int, Account> _accountRepository;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ITransactionRepository transactionRepository, IRepository<int, Account> accountRepository, IMapper mapper, ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TransactionResponseDTO> MakeTransaction(TransactionRequestDTO request)
        {
            _logger.LogInformation("Starting transaction for customer {CustomerId} with transaction type {TransactionTypeId} at {TransactionDate}.", request.CustomerId, request.TransactionTypeId, DateTime.UtcNow);

            var sourceAccount = await _transactionRepository.GetAccountByNumberAsync(request.SourceAccountNumber);
            Account destinationAccount = null;

            if (!string.IsNullOrEmpty(request.DestinationAccountNumber))
            {
                destinationAccount = await _transactionRepository.GetAccountByNumberAsync(request.DestinationAccountNumber);
            }

            var transactionType = await _transactionRepository.GetTransactionTypeByIdAsync(request.TransactionTypeId);

            try
            {
                switch (request.TransactionTypeId)
                {
                    case 1: // Withdraw
                        if (sourceAccount.Balance < request.Amount)
                            throw new InsufficientBalanceException("Insufficient balance for withdrawal.");
                        sourceAccount.Balance -= request.Amount;
                        _logger.LogInformation("Withdrawal of {Amount} successful for account {SourceAccountNumber}. New balance: {Balance}", request.Amount, request.SourceAccountNumber, sourceAccount.Balance);
                        break;

                    case 2: // Deposit
                        sourceAccount.Balance += request.Amount;
                        _logger.LogInformation("Deposit of {Amount} successful for account {SourceAccountNumber}. New balance: {Balance}", request.Amount, request.SourceAccountNumber, sourceAccount.Balance);
                        break;

                    case 3: // Transfer
                        if (sourceAccount.Balance < request.Amount)
                            throw new InsufficientBalanceException("Insufficient balance for transfer.");
                        if (destinationAccount == null)
                            throw new Exception("Destination account required for transfer.");
                        sourceAccount.Balance -= request.Amount;
                        destinationAccount.Balance += request.Amount;
                        _logger.LogInformation("Transfer of {Amount} from account {SourceAccountNumber} to {DestinationAccountNumber} successful.", request.Amount, request.SourceAccountNumber, request.DestinationAccountNumber);
                        break;

                    case 4: // Loan Repayment
                        if (sourceAccount.Balance < request.Amount)
                            throw new InsufficientBalanceException("Insufficient balance for loan repayment.");
                        sourceAccount.Balance -= request.Amount;
                        _logger.LogInformation("Loan repayment of {Amount} successful for account {SourceAccountNumber}. New balance: {Balance}", request.Amount, request.SourceAccountNumber, sourceAccount.Balance);
                        break;

                    default:
                        throw new Exception("Invalid transaction type.");
                }

                var transaction = new Transaction
                {
                    SourceAccountId = sourceAccount.AccountId,
                    DestinationAccountId = destinationAccount?.AccountId,
                    Amount = request.Amount,
                    TransactionTypeId = request.TransactionTypeId,
                    CustomerId = request.CustomerId,
                    TransactionDate = DateTime.UtcNow
                };

                await _transactionRepository.Add(transaction);
                await _accountRepository.Update(sourceAccount.AccountId, sourceAccount);

                if (destinationAccount != null)
                {
                    await _accountRepository.Update(destinationAccount.AccountId, destinationAccount);
                }

                var result = await _transactionRepository.GetById(transaction.TransactionId);
                _logger.LogInformation("Transaction {TransactionId} completed successfully for customer {CustomerId}.", result.TransactionId, request.CustomerId);

                return _mapper.Map<TransactionResponseDTO>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing transaction for customer {CustomerId} with transaction type {TransactionTypeId}.", request.CustomerId, request.TransactionTypeId);
                throw;
            }
        }

        public async Task<IEnumerable<TransactionResponseDTO>> GetAllTransactions()
        {
            var transactions = await _transactionRepository.GetAll();
            return _mapper.Map<IEnumerable<TransactionResponseDTO>>(transactions);
        }

        public async Task<TransactionResponseDTO> GetTransactionById(int id)
        {
            var transaction = await _transactionRepository.GetById(id);
            return _mapper.Map<TransactionResponseDTO>(transaction);
        }

        public async Task<IEnumerable<TransactionResponseDTO>> GetTransactionsByCustomerId(int customerId)
        {
            var transactions = await _transactionRepository.GetTransactionsByCustomerIdAsync(customerId);
            return _mapper.Map<IEnumerable<TransactionResponseDTO>>(transactions);
        }

        public async Task<IEnumerable<TransactionResponseDTO>> GetTransactionsByCustomerIdWithFilters(
            int customerId, int? transactionTypeId, DateTime? fromDate, DateTime? toDate)
        {
            var transactions = await _transactionRepository
                .GetTransactionsByCustomerWithFiltersAsync(customerId, transactionTypeId, fromDate, toDate);

            return _mapper.Map<IEnumerable<TransactionResponseDTO>>(transactions);
        }

        public async Task<IEnumerable<TransactionResponseDTO>> GetRecentTransactionsByCustomerId(int customerId)
        {
            var transactions = await _transactionRepository.GetTransactionsByCustomerIdAsync(customerId);
            var recent = transactions.OrderByDescending(t => t.TransactionDate).Take(3);
            return _mapper.Map<IEnumerable<TransactionResponseDTO>>(recent);
        }
    }
}
