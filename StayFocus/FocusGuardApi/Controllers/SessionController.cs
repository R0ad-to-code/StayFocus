using Microsoft.AspNetCore.Mvc;
using FocusGuardApi.DTOs;
using FocusGuardApi.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FocusGuardApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        /// <summary>
        /// Gets all focus sessions for the authenticated user
        /// </summary>
        /// <returns>A list of focus sessions</returns>
        /// <response code="200">Returns the list of sessions</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SessionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSessions()
        {
            var userId = GetUserId();
            var sessions = await _sessionService.GetAllSessionsForUserAsync(userId);
            return Ok(sessions);
        }

        /// <summary>
        /// Gets a specific focus session by ID
        /// </summary>
        /// <param name="id">The session ID</param>
        /// <returns>The focus session details</returns>
        /// <response code="200">Returns the session</response>
        /// <response code="404">If the session is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSession(int id)
        {
            var userId = GetUserId();
            var session = await _sessionService.GetSessionByIdAsync(id, userId);
            
            if (session == null)
            {
                return NotFound();
            }
            
            return Ok(session);
        }

        /// <summary>
        /// Creates a new focus session
        /// </summary>
        /// <param name="createDto">Session creation data</param>
        /// <returns>The created session</returns>
        /// <response code="201">Returns the newly created session</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPost]
        [ProducesResponseType(typeof(SessionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateSession([FromBody] SessionCreateDto createDto)
        {
            var userId = GetUserId();
            var createdSession = await _sessionService.CreateSessionAsync(userId, createDto);
            return CreatedAtAction(nameof(GetSession), new { id = createdSession.Id }, createdSession);
        }

        /// <summary>
        /// Updates an existing focus session
        /// </summary>
        /// <param name="id">The session ID</param>
        /// <param name="updateDto">Session update data</param>
        /// <returns>The updated session</returns>
        /// <response code="200">Returns the updated session</response>
        /// <response code="404">If the session is not found</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateSession(int id, [FromBody] SessionUpdateDto updateDto)
        {
            var userId = GetUserId();
            var updatedSession = await _sessionService.UpdateSessionAsync(id, userId, updateDto);
            
            if (updatedSession == null)
            {
                return NotFound();
            }
            
            return Ok(updatedSession);
        }

        /// <summary>
        /// Deletes a focus session
        /// </summary>
        /// <param name="id">The session ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the session was deleted successfully</response>
        /// <response code="404">If the session is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var userId = GetUserId();
            var result = await _sessionService.DeleteSessionAsync(id, userId);
            
            if (!result)
            {
                return NotFound();
            }
            
            return NoContent();
        }

        /// <summary>
        /// Starts or restarts a focus session
        /// </summary>
        /// <param name="id">The session ID</param>
        /// <returns>The started session</returns>
        /// <response code="200">Returns the started session</response>
        /// <response code="404">If the session is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPost("{id}/start")]
        [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> StartSession(int id)
        {
            var userId = GetUserId();
            var session = await _sessionService.StartSessionAsync(id, userId);
            
            if (session == null)
            {
                return NotFound();
            }
            
            return Ok(session);
        }

        /// <summary>
        /// Ends an active focus session
        /// </summary>
        /// <param name="id">The session ID</param>
        /// <param name="endDto">Session end data</param>
        /// <returns>The completed session</returns>
        /// <response code="200">Returns the completed session</response>
        /// <response code="404">If the session is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPost("{id}/end")]
        [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EndSession(int id, [FromBody] SessionEndDto endDto)
        {
            var userId = GetUserId();
            var session = await _sessionService.EndSessionAsync(id, userId, endDto);
            
            if (session == null)
            {
                return NotFound();
            }
            
            return Ok(session);
        }

        /// <summary>
        /// Gets all sessions within a specified date range
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>A list of sessions within the date range</returns>
        /// <response code="200">Returns the list of sessions</response>
        /// <response code="400">If the date range is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("daterange")]
        [ProducesResponseType(typeof(IEnumerable<SessionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSessionsInDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest(new { message = "Start date must be before end date" });
            }

            var userId = GetUserId();
            var sessions = await _sessionService.GetSessionsInDateRangeAsync(userId, startDate, endDate);
            return Ok(sessions);
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim);
        }
    }
}
