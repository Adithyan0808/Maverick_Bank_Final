namespace MaverickBank.Models.DTOs
{
    public class CustomerAccountDTO
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
