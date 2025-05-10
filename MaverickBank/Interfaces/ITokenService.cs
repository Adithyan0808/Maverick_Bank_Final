namespace MaverickBank.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(int id, string username, string role);
    }
}
