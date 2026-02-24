using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.DTOs.Groups;
using SplitWise.Application.DTOs.Sessions;
using SplitWise.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace Splitwise.API.Controllers
{
    public class LoginController : Controller
    {
       private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("api/login")]
        public async Task<IActionResult> CreateSession([FromBody] LoginRequestDto request)
        {
            var result = await _loginService.Login(request);

            result.meta = new MetaData
            {
                requestId = HttpContext.TraceIdentifier,
                timeStamp = System.DateTime.UtcNow.ToString("o")
            };

            return StatusCode(result.statusCode, result);
        }
    }
}
