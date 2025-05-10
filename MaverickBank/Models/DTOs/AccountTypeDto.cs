namespace MaverickBank.Models.DTOs
{
    public class AccountTypeDto
    {
        public int AccountTypeId { get; set; }
        public string AccountTypeName { get; set; } = string.Empty;
    }

    public class AccountTypeDropdownDto
    {
        public List<AccountTypeDto> AccountTypes { get; set; } = new List<AccountTypeDto>();
    }
}
