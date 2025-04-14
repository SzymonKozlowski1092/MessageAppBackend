using MessageAppBackend.Common.Helpers;
using MessageAppBackend.DTO;
using MessageAppBackend.Services;
using MessageAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessageAppBackend.Controllers
{
    [Route("MessageApp/api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IAccountService _accountService;
        ITokenService _tokenService;
        public AccountController(IAccountService accountService, ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
  
            var result = await _accountService.Register(registerUserDto);
            if (result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }
            
            return Ok();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var result = await _accountService.Login(loginRequestDto)!;
            if(result.IsFailed)
            {
                return ErrorMapper.MapErrorToResponse(result.Errors.First());
            }

            var token = _tokenService.GenerateJwtToken(result.Value);
            return Ok(new {Token = token});
        }
    }
}
