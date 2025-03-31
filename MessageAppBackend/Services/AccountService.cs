using Microsoft.AspNetCore.Identity;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO;
using Microsoft.EntityFrameworkCore;
using MessageAppBackend.Database;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MessageAppBackend.Services
{
    public interface IAccountService
    {
        public void RegisterUser(RegisterUserDto registerUserDto);
    }
    public class AccountService : IAccountService
    {
        private readonly MessageAppDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountService(MessageAppDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public void RegisterUser(RegisterUserDto registerUserDto)
        {
            if (registerUserDto is null)
                throw new ArgumentNullException(nameof(registerUserDto));

            var newLecturer = new User
            {
                Username = registerUserDto.Username,
                DisplayName = registerUserDto.DisplayName,
                Email = registerUserDto.Email,
            };

            newLecturer.PasswordHash = _passwordHasher.HashPassword(newLecturer, registerUserDto.Password);

            _dbContext.Users.Add(newLecturer);
            _dbContext.SaveChanges();
        }
    }
}
