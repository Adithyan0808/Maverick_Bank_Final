using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MaverickBank.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        [ForeignKey("SourceAccount")]
        public int? SourceAccountId { get; set; }

        [ForeignKey("DestinationAccount")]
        public int? DestinationAccountId { get; set; }

        [Required]
        [ForeignKey("TransactionType")]
        public int TransactionTypeId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }

        [ForeignKey("Employee")]
        public int? EmployeeId { get; set; }

        // Navigation properties
        public Account? SourceAccount { get; set; }
        public Account? DestinationAccount { get; set; }
        public TransactionType? TransactionType { get; set; }
        public Customer? Customer { get; set; }
        public Employee? Employee { get; set; }
    }

}
