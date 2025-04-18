using FluentResults;

namespace MessageAppBackend.Services.Interfaces
{
    public interface ICurrentUserService
    {
        public Result<Guid> GetUserId();
        public Result<string> GetUsername();
    }
}
