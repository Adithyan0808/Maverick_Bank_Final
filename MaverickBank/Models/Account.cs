using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Account
{
    public int AccountId { get; set; }

    [Required]
    public string AccountNumber { get; set; }

    public decimal Balance { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign Keys
    [ForeignKey("Customer")]
    public int CustomerId { get; set; }

    [ForeignKey("AccountType")]
    public int AccountTypeId { get; set; }

    // Navigation
    public Customer? Customer { get; set; }
    public AccountType? AccountType { get; set; }
}
