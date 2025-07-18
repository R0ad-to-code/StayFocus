using FocusGuardApi.DTOs;

namespace FocusGuardApi.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionDto>> GetAllSessionsForUserAsync(int userId);
        Task<SessionDto> GetSessionByIdAsync(int sessionId, int userId);
        Task<SessionDto> CreateSessionAsync(int userId, SessionCreateDto createDto);
        Task<SessionDto> UpdateSessionAsync(int sessionId, int userId, SessionUpdateDto updateDto);
        Task<bool> DeleteSessionAsync(int sessionId, int userId);
        Task<SessionDto> StartSessionAsync(int sessionId, int userId);
        Task<SessionDto> EndSessionAsync(int sessionId, int userId, SessionEndDto endDto);
        Task<IEnumerable<SessionDto>> GetSessionsInDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    }
}
