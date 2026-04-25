using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShortLink.Api.DTOs.Account;
using ShortLink.Application.Features.Account;
using ShortLink.Application.Features.Account.Login;
using ShortLink.Application.Features.Account.Register;

namespace ShortLink.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(request.UserName, request.Email, request.Password);
            var result = await _mediator.Send(command, cancellationToken);

            if (result is null)
            {
                return BadRequest(new { message = "Registration failed. Email may already be in use." });
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Login(
            [FromBody] LoginRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await _mediator.Send(command, cancellationToken);

            if (result is null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(result);
        }
    }
}
