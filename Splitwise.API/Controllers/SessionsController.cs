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
        public async Task<IActionResult> CreateSession([FromBody] SessionRequestDto request)
        {
            try
            {
                var meta = new
                {
                    requestId = HttpContext.TraceIdentifier,
                    timestamp = DateTime.UtcNow.ToString("o")
                };
                var result = await _sessionService.Login(request);
                if (result == null)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        success = false,
                        message = "User login failed",
                        data = (object)null,
                        errors = new[]
                        {
                            new
                            {
                                type = "Invalid_Credentials",
                                message = "Ivalid login credentials provided"
                            }
                        },
                        meta = meta
                    });
                }
                else
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        success = true,
                        message = "User login successful",
                        data = new
                        {
                            token = result.Token
                        },
                        errors = (object)null,
                        meta = meta
                    });
                }
            }
            catch(Exception ex)
            {
                return StatusCode(400, new { Success = false, Message = "Ivalid login credentials provided" });
            }
        }
    }
}
