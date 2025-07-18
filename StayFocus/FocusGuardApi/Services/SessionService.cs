using FocusGuardApi.DTOs;
using FocusGuardApi.Models;
using FocusGuardApi.Services.Interfaces;
using FocusGuardApi.Data;
using Microsoft.EntityFrameworkCore;

namespace FocusGuardApi.Services
{
    public class SessionService : ISessionService
    {
        private readonly ApplicationDbContext _context;

        public SessionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SessionDto>> GetAllSessionsForUserAsync(int userId)
        {
            var sessions = await _context.Sessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            return sessions.Select(MapToDto);
        }

        public async Task<SessionDto> GetSessionByIdAsync(int sessionId, int userId)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);

            if (session == null)
            {
                return null;
            }

            return MapToDto(session);
        }

        public async Task<SessionDto> CreateSessionAsync(int userId, SessionCreateDto createDto)
        {
            var session = new Session
            {
                UserId = userId,
                Name = createDto.Name,
                Description = createDto.Description,
                PlannedDurationMinutes = createDto.PlannedDurationMinutes,
                StartTime = DateTime.UtcNow,
                IsCompleted = false,
                Notes = string.Empty
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return MapToDto(session);
        }

        public async Task<SessionDto> UpdateSessionAsync(int sessionId, int userId, SessionUpdateDto updateDto)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);

            if (session == null)
            {
                return null;
            }

            // Update only the fields that are provided
            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                session.Name = updateDto.Name;
            }

            if (updateDto.Description != null) // Allow setting to empty string
            {
                session.Description = updateDto.Description;
            }

            if (updateDto.PlannedDurationMinutes.HasValue)
            {
                session.PlannedDurationMinutes = updateDto.PlannedDurationMinutes.Value;
            }

            if (updateDto.Notes != null) // Allow setting to empty string
            {
                session.Notes = updateDto.Notes;
            }

            await _context.SaveChangesAsync();
            return MapToDto(session);
        }

        public async Task<bool> DeleteSessionAsync(int sessionId, int userId)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);

            if (session == null)
            {
                return false;
            }

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SessionDto> StartSessionAsync(int sessionId, int userId)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);

            if (session == null)
            {
                return null;
            }

            // Reset the start time and end time
            session.StartTime = DateTime.UtcNow;
            session.EndTime = null;
            session.IsCompleted = false;
            session.ActualDurationMinutes = null;

            await _context.SaveChangesAsync();
            return MapToDto(session);
        }

        public async Task<SessionDto> EndSessionAsync(int sessionId, int userId, SessionEndDto endDto)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);

            if (session == null)
            {
                return null;
            }

            // Mark the session as completed
            session.EndTime = DateTime.UtcNow;
            session.IsCompleted = true;
            
            // Calculate actual duration in minutes
            TimeSpan duration = session.EndTime.Value - session.StartTime;
            session.ActualDurationMinutes = (int)duration.TotalMinutes;

            // Update notes if provided
            if (endDto?.Notes != null)
            {
                session.Notes = endDto.Notes;
            }

            await _context.SaveChangesAsync();
            return MapToDto(session);
        }

        public async Task<IEnumerable<SessionDto>> GetSessionsInDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var sessions = await _context.Sessions
                .Where(s => s.UserId == userId && s.StartTime >= startDate && s.StartTime <= endDate)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            return sessions.Select(MapToDto);
        }

        private static SessionDto MapToDto(Session session)
        {
            return new SessionDto
            {
                Id = session.Id,
                Name = session.Name,
                Description = session.Description,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                PlannedDurationMinutes = session.PlannedDurationMinutes,
                ActualDurationMinutes = session.ActualDurationMinutes,
                IsCompleted = session.IsCompleted,
                Notes = session.Notes
            };
        }
    }
}
