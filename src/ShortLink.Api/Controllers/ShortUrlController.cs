using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShortLink.Api.DTOs.ShortUrl;
using ShortLink.Application.Features.ShortUrl.Commands.CreateShortUrl;
using ShortLink.Application.Features.ShortUrl.Queries.GetAllAdmin;
using ShortLink.Application.Features.ShortUrl.Queries.GetAllByUserId;
using ShortLink.Application.Features.ShortUrl.Queries.GetById;

namespace ShortLink.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortUrlController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ShortUrlController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateShortUrl(CreateUrlDto createDto)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();
            var command = new CreateShortUrlCommand(userId, createDto.Url);

            var res = await _mediator.Send(command);

            if (res is null)
                return BadRequest();

            return CreatedAtAction(nameof(GetById), new { res.Id });
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(string id)
        {
            if (!Guid.TryParse(id, out Guid result))
                return BadRequest();

            var query = new GetByIdQuery(result);
            var res = await _mediator.Send(query);
            if (res is null)
                return NotFound();

            return Ok(res);
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllByUserId()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var query = new GetAllQuery(userId);
            var res = await _mediator.Send(query);

            return Ok(res);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var query = new GetAllAdminQuery();
            var res = await _mediator.Send(query);

            return Ok(res);
        }
        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out Guid result))
                return result;

            return Guid.Empty;
        }
    }
}
