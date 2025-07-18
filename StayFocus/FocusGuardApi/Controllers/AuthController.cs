using Microsoft.AspNetCore.Mvc;
using FocusGuardApi.DTOs;
using FocusGuardApi.Services.Interfaces;
using System.Security.Claims;

namespace FocusGuardApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="registerDto">User registration information</param>
        /// <returns>Authentication response with JWT token</returns>
        /// <response code="200">Returns the authentication response</response>
        /// <response code="400">If registration fails</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var (success, message, response) = await _authService.RegisterAsync(registerDto);
            
            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(response);
        }

        /// <summary>
        /// Logs in an existing user
        /// </summary>
        /// <param name="loginDto">User login information</param>
        /// <returns>Authentication response with JWT token</returns>
        /// <response code="200">Returns the authentication response</response>
        /// <response code="400">If login fails</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var (success, message, response) = await _authService.LoginAsync(loginDto);
            
            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(response);
        }

        /// <summary>
        /// Refreshes an expired JWT token
        /// </summary>
        /// <returns>New authentication response with fresh JWT token</returns>
        /// <response code="200">Returns the new authentication response</response>
        /// <response code="401">If refresh token is invalid</response>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var response = await _authService.RefreshTokenAsync(refreshToken);
            
            if (response == null)
            {
                return Unauthorized();
            }

            return Ok(response);
        }

        /// <summary>
        /// Gets the current authenticated user information
        /// </summary>
        /// <returns>User information</returns>
        /// <response code="200">Returns the user information</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userDto = new UserDto
            {
                Id = int.Parse(userId),
                Username = username,
                Email = email
            };

            return Ok(userDto);
        }
    }
}
