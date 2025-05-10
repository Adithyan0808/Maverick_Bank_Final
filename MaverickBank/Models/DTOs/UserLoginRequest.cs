using System.ComponentModel.DataAnnotations;

namespace MaverickBank.Models.DTOs
{
    public class UserLoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
