using MessageAppBackend.DTO;
using MessageAppBackend.Services;
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
        public AccountController(IAccountService accontService, ITokenService tokenService)
        {
            _accountService = accontService;
            _tokenService = tokenService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            await _accountService.Register(registerUserDto);
            return Ok();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loggedUser = await _accountService.Login(loginRequestDto)!;
            if(loggedUser is null)
            {
                return Unauthorized("Nieprawidłowa nazwa użytkownika lub hasło.");
            }

            var token = _tokenService.GenerateJwtToken(loggedUser);
            return Ok(new {Token = token});
        }
    }
}
