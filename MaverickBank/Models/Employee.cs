using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Employee
{
    public int EmployeeId { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    public string Email { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign Keys
    [ForeignKey("Branch")]
    public int BranchId { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    // Navigation
    public Branch? Branch { get; set; }
    public User? User { get; set; }
}
