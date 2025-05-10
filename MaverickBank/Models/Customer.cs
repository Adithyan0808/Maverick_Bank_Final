using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Customer
{
    public int CustomerId { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string Gender { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public string AadharNumber { get; set; }

    [Required]
    public string PANNumber { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign Key
    [ForeignKey("User")]
    public int UserId { get; set; }

    // Navigation
    public User? User { get; set; }
    public ICollection<Account>? Accounts { get; set; }
}
