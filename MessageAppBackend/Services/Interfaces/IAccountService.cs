using MessageAppBackend.DbModels;
using MessageAppBackend.DTO;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<bool> Register(RegisterUserDto registerUserDto);
        public Task<User>? Login(LoginRequestDto loginUserDto);
    }
}
