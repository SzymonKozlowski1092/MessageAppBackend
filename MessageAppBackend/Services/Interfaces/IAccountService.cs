using FluentResults;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO.AccountDTOs;

namespace MessageAppBackend.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<Result> Register(RegisterUserDto registerUserDto);
        public Task<Result<User>> Login(LoginRequestDto loginUserDto);
    }
}
