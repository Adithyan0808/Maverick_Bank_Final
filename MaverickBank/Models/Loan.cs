using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaverickBank.Models
{
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [Required]
        public int LoanMasterId { get; set; }

        [ForeignKey("LoanMasterId")]
        public LoanMaster? LoanMaster { get; set; }

        [Required]
        public decimal LoanAmount { get; set; }

        [Required]
        public LoanStatus LoanStatus { get; set; }

        public float? CustomInterestRate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum LoanStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
