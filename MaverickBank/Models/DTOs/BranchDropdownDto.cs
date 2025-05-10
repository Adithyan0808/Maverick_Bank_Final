namespace MaverickBank.Models.DTOs
{
    public class BranchDto
    {
        public int BranchId { get; set; }           // Match model
        public string BranchName { get; set; } = string.Empty;  // Match model
    }

    public class BranchDropdownDto
    {
        public List<BranchDto> Branches { get; set; } = new List<BranchDto>();
    }
}
