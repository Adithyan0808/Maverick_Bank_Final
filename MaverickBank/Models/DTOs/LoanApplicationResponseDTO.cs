namespace MaverickBank.Models.DTOs
{
    public class LoanApplicationResponseDTO
    {
        public int LoanId { get; set; }
        public string LoanTypeName { get; set; }
        public decimal LoanAmount { get; set; }
        public string LoanStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
