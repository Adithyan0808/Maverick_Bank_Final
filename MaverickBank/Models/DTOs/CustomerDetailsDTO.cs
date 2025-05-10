namespace MaverickBank.Models.DTOs
{
    public class CustomerDetailsDTO
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public List<AccountDTO> Accounts { get; set; }
    }
}
