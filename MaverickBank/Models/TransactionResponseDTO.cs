namespace MaverickBank.Models
{
    public class TransactionResponseDTO
    {
       
        public int TransactionId { get; set; }
        public string? SourceAccountNumber { get; set; }
        public string? DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }

    }

}
