namespace MaverickBank.Models.DTOs
{
    public class EmployeeDetailsDTO
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string BranchName { get; set; }

        //04.05.25
        public string PhoneNumber { get; set; }
        //end
    }
}
