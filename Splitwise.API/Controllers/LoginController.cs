using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Groups;
using SplitWise.Application.DTOs.Sessions;
using SplitWise.Application.DTOs.Common;
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
            if (result == null)
            {
                return Unauthorized(
                    ApiResponseFactory.Failure<object>(
                        message: "User login failed",
                        errorType: "Invalid_Credentials",
                        errorMessage: "Invalid login credentials provided",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status401Unauthorized
                    )
                );
            }
            else
            {
                return Ok(
                    ApiResponseFactory.Success(
                        data: new {token = result.Token},
                        message: "User login successful",
                        httpContext: HttpContext,
                        statusCode: StatusCodes.Status200OK
                    )
                );
            }
        }
    }
}
