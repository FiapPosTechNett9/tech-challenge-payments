namespace FIAP.CloudGames.Payments.Domain.Interfaces
{
    public interface IJwtTokenGenerator
    {
        public string GenerateToken(string userId, string email, string role);
    }
}
