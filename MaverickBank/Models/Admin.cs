using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Admin
{
    public int AdminId { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string Email { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign Key
    [ForeignKey("User")]
    public int UserId { get; set; }

    // Navigation
    public User? User { get; set; }
}
