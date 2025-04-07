using Microsoft.AspNetCore.Identity;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO;
using Microsoft.EntityFrameworkCore;
using MessageAppBackend.Database;
using MessageAppBackend.Services.Interfaces;

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

        public async Task<User>? Login(LoginRequestDto loginRequestDto)
        {
            if (loginRequestDto is null)
                throw new ArgumentNullException(nameof(loginRequestDto));

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == loginRequestDto.Username);
            if(user is null)
            {
                return null!;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequestDto.Password);
            if(result == PasswordVerificationResult.Failed)
            {
                return null!;
            }

            return user;
        }

        public async Task<bool> Register(RegisterUserDto registerUserDto)
        {
            if (registerUserDto is null)
                throw new ArgumentNullException(nameof(registerUserDto));

            bool userExists = await _dbContext.Users.AnyAsync(u => u.Email == registerUserDto.Email);
            if (userExists) 
            {
                return false;
            }

            var newLecturer = new User
            {
                Username = registerUserDto.Username,
                DisplayName = registerUserDto.DisplayName,
                Email = registerUserDto.Email,
            };

            newLecturer.PasswordHash = _passwordHasher.HashPassword(newLecturer, registerUserDto.Password);

            _dbContext.Users.Add(newLecturer);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
