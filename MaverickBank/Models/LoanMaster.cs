using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaverickBank.Models
{
    public class LoanMaster
    {
        [Key]
        public int LoanMasterId { get; set; }

        [Required]
        public string LoanTypeName { get; set; } // CarLoan, HomeLoan, etc.

        [Required]
        public decimal MaxLoanAmount { get; set; }

        [Required]
        public float DefaultInterestRate { get; set; }

        // Optional: Add navigation property for all loans of this type
        public ICollection<Loan>? Loans { get; set; }
    }
}
