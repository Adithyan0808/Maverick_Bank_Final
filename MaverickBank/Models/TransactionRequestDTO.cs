namespace MaverickBank.Models
{
    public class TransactionRequestDTO
    {
       
        public int TransactionTypeId { get; set; }
        public string SourceAccountNumber { get; set; } = string.Empty;
        public string? DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public int CustomerId { get; set; }
    }

}
