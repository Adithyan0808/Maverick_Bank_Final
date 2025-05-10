using System.ComponentModel.DataAnnotations;

public class AccountType
{
    public int AccountTypeId { get; set; }

    [Required]
    public string AccountTypeName { get; set; } // e.g., Savings, Current

    
    public ICollection<Account>? Accounts { get; set; }
}
