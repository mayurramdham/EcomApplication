using App.core.App.User.Command;
using App.core.App.User.Query;
using App.core.Model.Register;
using Domain.Entity.Register;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace EComApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserDto user, IFormFile profileImage)
        {
            var userId = await _mediator.Send(new RegisterUserCommand { UserDto= user });
            return Ok(userId);
        }


        [HttpPost("Otp")]
        public async Task<IActionResult> Otp([FromBody] LoginDto logins)
        {
            var otp = await _mediator.Send(new SendOtpCommand { LoginDto = logins });
            return Ok(otp);
        }


        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] OtpDto otp)       
        {
            var login = await _mediator.Send(new LoginOtpCommand { otpDto = otp });
            return Ok(login);
        }


        [HttpGet("ForgetPassword/{email}")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var response = await _mediator.Send(new ForgotPasswordCommand {Email= email } );
            return Ok(response);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto command)
        {
            var response = await _mediator.Send(new ChangePasswordCommand { ChangePasswordDto = command });
            return Ok(response);
        }
        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto updateDto)
        {
            var updateUser = await _mediator.Send(new UpdateUserCommand { userDto = updateDto });
            return Ok(updateUser);
        }

        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> UserById(int id)
        {
            var response = await _mediator.Send(new GetUserByIdQuery { UserId = id });
            return Ok(response);
        }


    }
}
