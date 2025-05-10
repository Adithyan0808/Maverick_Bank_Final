using System.ComponentModel.DataAnnotations;

public class User
{
    public int UserId { get; set; } // PK

    [Required]
    public string Username { get; set; }

    [Required]
    public string PasswordHash { get; set; } // hashed password

    [Required]
    public string Role { get; set; } // Admin, Customer, Employee

    // Navigation
    public Customer? Customer { get; set; }
    public Employee? Employee { get; set; }
    public Admin? Admin { get; set; }

}
