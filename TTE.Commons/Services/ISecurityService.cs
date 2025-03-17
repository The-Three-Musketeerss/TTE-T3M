namespace TTE.Commons.Services
{
    public interface ISecurityService
    {
        string GenerateToken(string username, string role, int id);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);

    }
}
