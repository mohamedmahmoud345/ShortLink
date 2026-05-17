using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShortLink.Api.DTOs.ShortUrl;
using ShortLink.Application.Features.Admin.Queries.GetAllUsers;
using ShortLink.Application.Features.ShortUrl.Commands.CreateShortUrl;
using ShortLink.Application.Features.ShortUrl.Commands.DeleteUrl;
using ShortLink.Application.Features.ShortUrl.Commands.UpdateShortUrl;
using ShortLink.Application.Features.ShortUrl.Queries.GetAllByUserId;
using ShortLink.Application.Features.ShortUrl.Queries.GetById;
using ShortLink.Application.Features.ShortUrl.Queries.GetByShortCode;

namespace ShortLink.Api.Controllers
{
    [Authorize]
    [EnableRateLimiting("PerUserPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShortUrlController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ShortUrlController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateShortUrl([FromBody] CreateUrlDto createDto)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();
            var command = new CreateShortUrlCommand(userId, createDto.Url);

            var res = await _mediator.Send(command);

            if (res is null)
                return BadRequest();

            return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);
        }

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
        [HttpGet("admin/users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllUsers()
        {
            var query = new GetAllUsersQuery();
            var res = await _mediator.Send(query);

            return Ok(res);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/user/{userId}/urls")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllByUserIdAdmin([FromRoute] string userId)
        {
            if (!Guid.TryParse(userId, out Guid result))
                return BadRequest();

            var query = new GetAllQuery(result);
            var res = await _mediator.Send(query);

            return Ok(res);
        }

        [HttpGet("url/{shortCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByShortCode([FromRoute] string shortCode)
        {
            var query = new GetByShortCodeQuery(shortCode);
            var res = await _mediator.Send(query);
            if (res is null)
                return NotFound();

            return Ok(res);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(UpdateDto updateDto)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();
            var command = new UpdateCommand(updateDto.Id, userId, updateDto.Url);
            var res = await _mediator.Send(command);
            if (res == false)
                return NotFound();

            return NoContent();
        }
        
        [HttpDelete("{urlId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid urlId)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var command = new DeleteCommand(urlId, userId);
            var res = await _mediator.Send(command);
            if (res == false)
                return NotFound();

            return NoContent();
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
