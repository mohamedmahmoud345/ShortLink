using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShortLink.Api.DTOs.ClickEvent;
using ShortLink.Api.Filters;
using ShortLink.Application.Features.ClickEvent.Commands.RecordClickEvent;

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
    }
}
