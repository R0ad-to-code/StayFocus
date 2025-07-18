using Microsoft.AspNetCore.Mvc;
using FocusGuardApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace FocusGuardApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MotivationController : ControllerBase
    {
        private readonly IMotivationService _motivationService;

        public MotivationController(IMotivationService motivationService)
        {
            _motivationService = motivationService;
        }

        /// <summary>
        /// Gets a random motivational quote
        /// </summary>
        /// <returns>A motivational quote</returns>
        /// <response code="200">Returns a motivational quote</response>
        [HttpGet("quote")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRandomQuote()
        {
            var quote = await _motivationService.GetRandomQuoteAsync();
            return Ok(new { quote });
        }

        /// <summary>
        /// Gets a personalized motivational message based on session count
        /// </summary>
        /// <param name="sessionCount">The number of completed sessions</param>
        /// <returns>A personalized motivational message</returns>
        /// <response code="200">Returns a motivational message</response>
        /// <response code="400">If the session count is negative</response>
        [HttpGet("message")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMotivationalMessage([FromQuery] int sessionCount)
        {
            if (sessionCount < 0)
            {
                return BadRequest(new { message = "Session count cannot be negative" });
            }

            var motivationalMessage = await _motivationService.GetMotivationalMessageAsync(sessionCount);
            return Ok(new { message = motivationalMessage });
        }
    }
}
