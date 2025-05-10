namespace MaverickBank.Models.DTOs
{
    public class CustomerSelfDetailsDTO
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<CustomerAccountDTO> Accounts { get; set; } = new();
    }
}
