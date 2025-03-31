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
        public AccountController(IAccountService accontService)
        {
            _accountService = accontService;
        }

        [HttpPost("Register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _accountService.RegisterUser(registerUserDto);
            return Ok();
        }
    }
}
