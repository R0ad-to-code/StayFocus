using FocusGuardApi.DTOs;

namespace FocusGuardApi.Services.Interfaces
{
    public interface IBlacklistService
    {
        Task<IEnumerable<BlacklistItemDto>> GetAllBlacklistItemsForUserAsync(int userId);
        Task<BlacklistItemDto> GetBlacklistItemByIdAsync(int itemId, int userId);
        Task<BlacklistItemDto> CreateBlacklistItemAsync(int userId, BlacklistItemCreateDto createDto);
        Task<BlacklistItemDto> UpdateBlacklistItemAsync(int itemId, int userId, BlacklistItemUpdateDto updateDto);
        Task<bool> DeleteBlacklistItemAsync(int itemId, int userId);
        Task<bool> IsUrlBlacklistedAsync(int userId, string url);
    }
}
