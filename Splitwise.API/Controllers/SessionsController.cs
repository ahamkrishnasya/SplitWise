using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Sessions;
using SplitWise.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace Splitwise.API.Controllers
{
    public class SessionsController : Controller
    {
       private readonly ISessionService _sessionService;
        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpPost("sessions")]
        public async Task<IActionResult> CreateSession(SessionRequestDto request)
        {
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if(user == null || user.Password != request.Password)
            {
                return Unauthorized();
            }
            return Ok();
        }
        [HttpDelete("sessions")]
        public IActionResult DeleteSession()
        {
            return Ok();
        }
    }
}
