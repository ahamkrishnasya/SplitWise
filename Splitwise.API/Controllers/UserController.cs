using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Users;
using SplitWise.Application.Interfaces.Services;

namespace Splitwise.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto request)
        {
            var (data, errors) = await _userService.RegisterAsync(request);

            var meta = new
            {
                requestId = Guid.NewGuid().ToString(),
                timestamp = DateTime.UtcNow.ToString("o")
            };

            if (data != null)
            {
                return StatusCode(201, new
                {
                    statusCode = 201,
                    success = true,
                    message = "Registration successful.",
                    data,
                    errors = (object?)null,
                    meta
                });
            }
            else if (errors.Contains(("EMAIL_ALREADY_EXISTS", "An account with this email already exists")))
                return Conflict(new
                {
                    statusCode = 409,
                    success = false,
                    message = "User with the provided email already exists",
                    data = (object?)null,
                    errors = errors?.Select(e => new { type = e.Type, message = e.Message }),
                    meta
                });
            else
                return BadRequest(new
                {
                    statusCode = 400,
                    success = false,
                    message = "User registration failed",
                    data = (object?)null,
                    errors = errors?.Select(e => new { type = e.Type, message = e.Message }),
                    meta
                });
        }
    }
}
