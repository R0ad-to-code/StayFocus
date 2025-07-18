using FocusGuardApi.DTOs;
using FocusGuardApi.Models;
using FocusGuardApi.Services.Interfaces;
using FocusGuardApi.Data;
using Microsoft.EntityFrameworkCore;

namespace FocusGuardApi.Services
{
    public class BlacklistService : IBlacklistService
    {
        private readonly ApplicationDbContext _context;

        public BlacklistService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlacklistItemDto>> GetAllBlacklistItemsForUserAsync(int userId)
        {
            var items = await _context.BlacklistItems
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.Name)
                .ThenBy(b => b.Url)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<BlacklistItemDto> GetBlacklistItemByIdAsync(int itemId, int userId)
        {
            var item = await _context.BlacklistItems
                .FirstOrDefaultAsync(b => b.Id == itemId && b.UserId == userId);

            if (item == null)
            {
                return null;
            }

            return MapToDto(item);
        }

        public async Task<BlacklistItemDto> CreateBlacklistItemAsync(int userId, BlacklistItemCreateDto createDto)
        {
            // Check if the URL is already blacklisted by the user
            var exists = await _context.BlacklistItems
                .AnyAsync(b => b.UserId == userId && b.Url == createDto.Url);

            if (exists)
            {
                return null;
            }

            var item = new BlacklistItem
            {
                UserId = userId,
                Url = createDto.Url,
                Name = createDto.Name,
                Reason = createDto.Reason,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.BlacklistItems.Add(item);
            await _context.SaveChangesAsync();

            return MapToDto(item);
        }

        public async Task<BlacklistItemDto> UpdateBlacklistItemAsync(int itemId, int userId, BlacklistItemUpdateDto updateDto)
        {
            var item = await _context.BlacklistItems
                .FirstOrDefaultAsync(b => b.Id == itemId && b.UserId == userId);

            if (item == null)
            {
                return null;
            }

            // Update only the fields that are provided
            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                item.Name = updateDto.Name;
            }

            if (updateDto.Reason != null) // Allow setting to empty string
            {
                item.Reason = updateDto.Reason;
            }

            if (updateDto.IsActive.HasValue)
            {
                item.IsActive = updateDto.IsActive.Value;
            }

            await _context.SaveChangesAsync();
            return MapToDto(item);
        }

        public async Task<bool> DeleteBlacklistItemAsync(int itemId, int userId)
        {
            var item = await _context.BlacklistItems
                .FirstOrDefaultAsync(b => b.Id == itemId && b.UserId == userId);

            if (item == null)
            {
                return false;
            }

            _context.BlacklistItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsUrlBlacklistedAsync(int userId, string url)
        {
            // Check if URL matches any active blacklist items for the user
            return await _context.BlacklistItems
                .AnyAsync(b => b.UserId == userId && b.IsActive && url.Contains(b.Url));
        }

        private static BlacklistItemDto MapToDto(BlacklistItem item)
        {
            return new BlacklistItemDto
            {
                Id = item.Id,
                Url = item.Url,
                Name = item.Name,
                Reason = item.Reason,
                CreatedAt = item.CreatedAt,
                IsActive = item.IsActive
            };
        }
    }
}
