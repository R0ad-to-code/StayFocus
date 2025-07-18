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
    public class BlacklistController : ControllerBase
    {
        private readonly IBlacklistService _blacklistService;

        public BlacklistController(IBlacklistService blacklistService)
        {
            _blacklistService = blacklistService;
        }

        /// <summary>
        /// Gets all blacklisted items for the authenticated user
        /// </summary>
        /// <returns>A list of blacklisted items</returns>
        /// <response code="200">Returns the list of blacklisted items</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlacklistItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBlacklistItems()
        {
            var userId = GetUserId();
            var items = await _blacklistService.GetAllBlacklistItemsForUserAsync(userId);
            return Ok(items);
        }

        /// <summary>
        /// Gets a specific blacklisted item by ID
        /// </summary>
        /// <param name="id">The blacklisted item ID</param>
        /// <returns>The blacklisted item details</returns>
        /// <response code="200">Returns the blacklisted item</response>
        /// <response code="404">If the item is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BlacklistItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBlacklistItem(int id)
        {
            var userId = GetUserId();
            var item = await _blacklistService.GetBlacklistItemByIdAsync(id, userId);
            
            if (item == null)
            {
                return NotFound();
            }
            
            return Ok(item);
        }

        /// <summary>
        /// Creates a new blacklisted item
        /// </summary>
        /// <param name="createDto">Blacklisted item creation data</param>
        /// <returns>The created blacklisted item</returns>
        /// <response code="201">Returns the newly created blacklisted item</response>
        /// <response code="400">If the request data is invalid or the URL is already blacklisted</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPost]
        [ProducesResponseType(typeof(BlacklistItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateBlacklistItem([FromBody] BlacklistItemCreateDto createDto)
        {
            var userId = GetUserId();
            var createdItem = await _blacklistService.CreateBlacklistItemAsync(userId, createDto);
            
            if (createdItem == null)
            {
                return BadRequest(new { message = "URL is already blacklisted" });
            }
            
            return CreatedAtAction(nameof(GetBlacklistItem), new { id = createdItem.Id }, createdItem);
        }

        /// <summary>
        /// Updates an existing blacklisted item
        /// </summary>
        /// <param name="id">The blacklisted item ID</param>
        /// <param name="updateDto">Blacklisted item update data</param>
        /// <returns>The updated blacklisted item</returns>
        /// <response code="200">Returns the updated blacklisted item</response>
        /// <response code="404">If the item is not found</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BlacklistItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateBlacklistItem(int id, [FromBody] BlacklistItemUpdateDto updateDto)
        {
            var userId = GetUserId();
            var updatedItem = await _blacklistService.UpdateBlacklistItemAsync(id, userId, updateDto);
            
            if (updatedItem == null)
            {
                return NotFound();
            }
            
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a blacklisted item
        /// </summary>
        /// <param name="id">The blacklisted item ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the item was deleted successfully</response>
        /// <response code="404">If the item is not found</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteBlacklistItem(int id)
        {
            var userId = GetUserId();
            var result = await _blacklistService.DeleteBlacklistItemAsync(id, userId);
            
            if (!result)
            {
                return NotFound();
            }
            
            return NoContent();
        }

        /// <summary>
        /// Checks if a URL is blacklisted for the authenticated user
        /// </summary>
        /// <param name="url">The URL to check</param>
        /// <returns>True if the URL is blacklisted, false otherwise</returns>
        /// <response code="200">Returns whether the URL is blacklisted</response>
        /// <response code="400">If the URL is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("check")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> IsUrlBlacklisted([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest(new { message = "URL cannot be empty" });
            }

            var userId = GetUserId();
            var isBlacklisted = await _blacklistService.IsUrlBlacklistedAsync(userId, url);
            return Ok(isBlacklisted);
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim);
        }
    }
}
