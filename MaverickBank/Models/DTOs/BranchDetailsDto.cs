namespace MaverickBank.Models.DTOs
{
    public class BranchDetailsDto
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
