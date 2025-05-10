namespace MaverickBank.Models.DTOs
{
    public class LoanApplicationDTO
    {
        public int CustomerId { get; set; }
        public int LoanMasterId { get; set; } // Refers to LoanType (CarLoan, HomeLoan, etc.)
        public decimal LoanAmount { get; set; }
    }
}
