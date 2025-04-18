using System.Security.Claims;
using FluentResults;
using MessageAppBackend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace MessageAppBackend.Services
{
    public class CurrentUserService :ICurrentUserService
    {
        IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Result<Guid> GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirst("UserId")?.Value;
            
            if (Guid.TryParse(userIdClaim, out var guid))
            {
                return Result.Ok(guid);
            }
            else
            {
                return Result.Fail(new Error("UserId claim is missing or invalid")
                    .WithMetadata("Code", "Unauthorized"));
            }
        }

        public Result<string> GetUsername()
        {
            string username = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if(username.IsNullOrEmpty())
            {
                return Result.Fail(new Error("Username claim is missing or invalid")
                    .WithMetadata("Code", "Unauthorized"));
            }
            else
            {
                return Result.Ok(username);
            }
        }
    }
}
