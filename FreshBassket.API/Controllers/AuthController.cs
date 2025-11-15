using FreshBasket.Application.Features.Authentication.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreshBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await _mediator.Send(command);

            if (response.Success)
                return Ok(response);
            else
                return Unauthorized(response);
        }

        [HttpPost("adminregister")]
        public async Task<IActionResult> AdminRegister([FromBody] AdminRegisterCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(new { Token = token });
        }
    }
}
