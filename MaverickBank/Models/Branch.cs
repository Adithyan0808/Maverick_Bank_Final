using System.ComponentModel.DataAnnotations;

public class Branch
{
    public int BranchId { get; set; }

    [Required]
    public string BranchName { get; set; }

    [Required]
    public string Location { get; set; }

    // Navigation
    public ICollection<Employee>? Employees { get; set; }
}
