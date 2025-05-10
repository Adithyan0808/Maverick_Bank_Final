//namespace MaverickBank.Models.DTOs
//{
//    public class LoginResponse
//    {
//        public int Id { get; set; }
//        public string Username { get; set; }
//        public string Role { get; set; }
//        public string Token { get; set; }
//    }
//}
namespace MaverickBank.Models.DTOs
{
    public class LoginResponse
    {
        public int Id { get; set; } // This will be customerId for customers, userId for others
        public string Username { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
