using MessageAppBackend.DbModels;

namespace MessageAppBackend.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
    }
}
