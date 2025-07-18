using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FocusGuardApi.Services.Interfaces;
using FocusGuardApi.Data;
using Microsoft.EntityFrameworkCore;

namespace FocusGuardApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets productivity statistics for the authenticated user
        /// </summary>
        /// <returns>Productivity statistics</returns>
        /// <response code="200">Returns the statistics</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStats()
        {
            var userId = GetUserId();
            
            // Get all completed sessions for the user
            var sessions = await _context.Sessions
                .Where(s => s.UserId == userId && s.IsCompleted)
                .ToListAsync();

            // Calculate statistics
            int totalSessions = sessions.Count;
            int totalMinutes = sessions.Sum(s => s.ActualDurationMinutes ?? 0);
            double averageSessionLength = totalSessions > 0 
                ? sessions.Average(s => s.ActualDurationMinutes ?? 0) 
                : 0;
            
            // Sessions in the last week
            var lastWeek = DateTime.UtcNow.AddDays(-7);
            int sessionsLastWeek = sessions.Count(s => s.StartTime >= lastWeek);
            
            // Sessions by day of week
            var sessionsByDayOfWeek = sessions
                .GroupBy(s => s.StartTime.DayOfWeek)
                .Select(g => new 
                {
                    DayOfWeek = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();
            
            // Most productive day
            var mostProductiveDay = sessionsByDayOfWeek
                .OrderByDescending(s => s.Count)
                .FirstOrDefault();

            return Ok(new 
            {
                TotalSessions = totalSessions,
                TotalMinutes = totalMinutes,
                AverageSessionLengthMinutes = Math.Round(averageSessionLength, 2),
                SessionsLastWeek = sessionsLastWeek,
                SessionsByDayOfWeek = sessionsByDayOfWeek,
                MostProductiveDay = mostProductiveDay?.DayOfWeek ?? "N/A"
            });
        }

        /// <summary>
        /// Gets statistics for a specific date range
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>Statistics for the date range</returns>
        /// <response code="200">Returns the statistics</response>
        /// <response code="400">If the date range is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("daterange")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStatsForDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest(new { message = "Start date must be before end date" });
            }

            var userId = GetUserId();
            
            // Get completed sessions in the date range
            var sessions = await _context.Sessions
                .Where(s => s.UserId == userId && 
                       s.IsCompleted && 
                       s.StartTime >= startDate && 
                       s.StartTime <= endDate)
                .ToListAsync();

            // Calculate statistics
            int totalSessions = sessions.Count;
            int totalMinutes = sessions.Sum(s => s.ActualDurationMinutes ?? 0);
            double averageSessionLength = totalSessions > 0 
                ? sessions.Average(s => s.ActualDurationMinutes ?? 0) 
                : 0;
            
            // Sessions per day
            var sessionsPerDay = sessions
                .GroupBy(s => s.StartTime.Date)
                .Select(g => new 
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Count = g.Count(),
                    TotalMinutes = g.Sum(s => s.ActualDurationMinutes ?? 0)
                })
                .OrderBy(s => s.Date)
                .ToList();

            return Ok(new 
            {
                TotalSessions = totalSessions,
                TotalMinutes = totalMinutes,
                AverageSessionLengthMinutes = Math.Round(averageSessionLength, 2),
                SessionsPerDay = sessionsPerDay,
                DateRange = new 
                {
                    StartDate = startDate.ToString("yyyy-MM-dd"),
                    EndDate = endDate.ToString("yyyy-MM-dd")
                }
            });
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim);
        }
    }
}
