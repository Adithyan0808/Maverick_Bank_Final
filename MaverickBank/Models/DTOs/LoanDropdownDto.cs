namespace MaverickBank.Models.DTOs
{
    public class LoanMasterDto
    {
        public int LoanMasterId { get; set; }
        public string LoanTypeName { get; set; } = string.Empty;
    }

    public class LoanDropdownDto
    {
        public List<LoanMasterDto> LoanMasters { get; set; } = new List<LoanMasterDto>();
    }
}
