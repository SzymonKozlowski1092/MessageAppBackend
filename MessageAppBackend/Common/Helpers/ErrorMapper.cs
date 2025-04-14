using FluentResults;
using MessageAppBackend.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Common.Helpers
{
    public class ErrorMapper
    {
        public static ActionResult MapErrorToResponse(IError error)
        {
            var code = error.Metadata.TryGetValue("Code", out var value) ? (ErrorCode)value : ErrorCode.Unknown;

            return code switch
            {
                ErrorCode.InvalidInput => new BadRequestObjectResult(error.Message),
                ErrorCode.AlreadyExists => new ConflictObjectResult(error.Message),
                ErrorCode.NotFound => new NotFoundObjectResult(error.Message),
                ErrorCode.Conflict => new ConflictObjectResult(error.Message),
                ErrorCode.Unauthorized => new UnauthorizedObjectResult(error.Message),
                ErrorCode.Forbidden => new ForbidResult(),
                ErrorCode.FailedOperation => new StatusCodeResult(500),
                ErrorCode.AuthenticationFailed => new UnauthorizedObjectResult(error.Message),
                _ => new BadRequestObjectResult(error.Message)
            };
        }
    }
}
