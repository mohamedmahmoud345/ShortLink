using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShortLink.Api.DTOs.ClickEvent;
using ShortLink.Api.Filters;
using ShortLink.Application.Features.ClickEvent.Commands.RecordClickEvent;
using ShortLink.Application.Features.ClickEvent.Queries.GetByUrlId;
using ShortLink.Application.Features.ClickEvent.Queries.GetCountryStats;
using ShortLink.Application.Features.ClickEvent.Queries.GetDailyClicks;
using ShortLink.Application.Features.ClickEvent.Queries.GetDeviceStats;
using ShortLink.Application.Features.ClickEvent.Queries.GetTopReferrers;

namespace ShortLink.Api.Controllers
{
    // [EnableRateLimiting("PerUserPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClickEventController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClickEventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [InternalOnly]
        [HttpPost]
        public async Task<IActionResult> RecordClickEvent(RecordDto recordDto)
        {
            var command = new RecordClickEventCommand(
                recordDto.ShortCode, recordDto.Referrer, recordDto.IpAddress, recordDto.UserAgent
            );

            var result = await _mediator.Send(command);

            if (!result) return NotFound();
            return Ok(new { Success = true });
        }

        [EnableRateLimiting("PerUserPolicy")]
        [Authorize]
        [HttpGet("{urlId}")]
        public async Task<IActionResult> GetByUrlId([FromRoute] Guid urlId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();
            if (Guid.Empty == userId)
                return Unauthorized();

            var query = new GetByUrlIdQuery(userId, urlId, page, pageSize);

            var res = await _mediator.Send(query);

            return Ok(res);
        }

        [EnableRateLimiting("PerUserPolicy")]
        [Authorize]
        [HttpGet("{urlId}/daily")]
        public async Task<IActionResult> GetDailyClicksByUrlId([FromRoute] Guid urlId, [FromQuery] DateTime? date)
        {
            var userId = GetUserId();
            if (Guid.Empty == userId)
                return Unauthorized();
                
            var query = new GetDailyClicksQuery(userId, urlId, date);

            var res = await _mediator.Send(query);

            return Ok(res);
        }

        [EnableRateLimiting("PerUserPolicy")]
        [Authorize]
        [HttpGet("{urlId}/top-referrers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTopReferrers(Guid urlId, [FromQuery] int limit)
        {
            var userId = GetUserId();
            if (Guid.Empty == userId)
                return Unauthorized();

            var query = new GetTopReferrersQuery(userId, urlId, limit);
            var res = await _mediator.Send(query);

            return Ok(res);            
        }


        [EnableRateLimiting("PerUserPolicy")]
        [Authorize]
        [HttpGet("{urlId}/country-stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCountryStats(Guid urlId)
        {
            var userId = GetUserId();
            if (Guid.Empty == userId)
                return Unauthorized();

            var query = new GetCountryStatsQuery(userId, urlId);
            var res = await _mediator.Send(query);

            return Ok(res);
        }

        [EnableRateLimiting("PerUserPolicy")]
        [Authorize]
        [HttpGet("{urlId}/device-stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDeviceStats(Guid urlId)
        {
            var userId = GetUserId();
            if (Guid.Empty == userId)
                return Unauthorized();

            var query = new GetDeviceStatsQuery(userId, urlId);
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
