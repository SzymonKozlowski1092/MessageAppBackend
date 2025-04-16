using Microsoft.AspNetCore.Identity;
using MessageAppBackend.DbModels;
using Microsoft.EntityFrameworkCore;
using MessageAppBackend.Database;
using MessageAppBackend.Services.Interfaces;
using FluentResults;
using MessageAppBackend.Common.Enums;
using MessageAppBackend.DTO.AccountDTOs;

namespace MessageAppBackend.Services
{
    public class AccountService : IAccountService
    {
        private readonly MessageAppDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountService(MessageAppDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<User>> Login(LoginRequestDto loginRequestDto)
        {
            if (loginRequestDto is null)
                throw new ArgumentNullException(nameof(loginRequestDto));

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == loginRequestDto.Username);
            if(user is null)
            {
                return Result.Fail(new Error("Incorrect username or password")
                    .WithMetadata("Code", ErrorCode.AuthenticationFailed));
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequestDto.Password);
            if(result == PasswordVerificationResult.Failed)
            {
                return Result.Fail(new Error("Incorrect username or password")
                    .WithMetadata("Code", ErrorCode.AuthenticationFailed));
            }

            return Result.Ok(user);
        }

        public async Task<Result> Register(RegisterUserDto registerUserDto)
        {
            if (registerUserDto is null)
                throw new ArgumentNullException(nameof(registerUserDto));

            bool isExistsUserWithSameEmail = await _dbContext.Users.AnyAsync(u => u.Email == registerUserDto.Email);
            if (isExistsUserWithSameEmail) 
            {
                return Result.Fail(new Error($"User with email: {registerUserDto.Email} already exists")
                    .WithMetadata("Code", ErrorCode.AlreadyExists));
            }

            bool isExistsUserWithSameUsername = await _dbContext.Users.AnyAsync(u => u.Username == registerUserDto.Username);
            if (isExistsUserWithSameUsername)
            {
                return Result.Fail(new Error($"User with username: {registerUserDto.Username} already exists")
                    .WithMetadata("Code", ErrorCode.AlreadyExists));
            }

            var newUser = new User
            {
                Username = registerUserDto.Username,
                DisplayName = registerUserDto.DisplayName,
                Email = registerUserDto.Email,
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, registerUserDto.Password);

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
