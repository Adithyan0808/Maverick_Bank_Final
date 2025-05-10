using System.ComponentModel.DataAnnotations;

namespace MaverickBank.Models
{
    public class TransactionType
    {
        public int TransactionTypeId { get; set; }

        [Required]
        public string TransactionTypeName { get; set; } = string.Empty;

        // Navigation
        public ICollection<Transaction>? Transactions { get; set; } = new List<Transaction>();
    }

}
