namespace MaverickBank.Models.DTOs
{
    public class TransactionTypeDto
    {
        public int TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; } = string.Empty;
    }

    public class TransactionTypeDropdownDto
    {
        public List<TransactionTypeDto> TransactionTypes { get; set; } = new List<TransactionTypeDto>();
    }
}
