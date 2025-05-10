using System.ComponentModel.DataAnnotations;

namespace MaverickBank.Models.DTOs
{
    public class RegisterCustomerDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; } // plain password, we’ll hash it

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

        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public int AccountTypeId { get; set; } // selected by customer
    }
}
