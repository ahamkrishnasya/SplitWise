using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SplitWise.Application.DTOs.Common;
using SplitWise.Application.DTOs.Sessions;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SplitWise.Application.Services
{
    public class LoginService: ILoginService
    {
        private readonly ILoginRepository _loginRepository; 
        private readonly IConfiguration _configuration;
        public LoginService(ILoginRepository loginRepository, IConfiguration Configuration, IUserRepository userRepository)
        {
            _loginRepository = loginRepository;
            _configuration = Configuration;
        }

        public async Task<ApiResponse<object>> Login(LoginRequestDto request)
        {
            if(string.IsNullOrWhiteSpace(request.Email))
            {
                return ApiResponseFactory.Failure<object>(
                        message: "User login failed",
                        errorType: "Email_Missing",
                        errorMessage: "Email is required",
                        statusCode: StatusCodes.Status400BadRequest
                );
            }

            if(string.IsNullOrWhiteSpace(request.Password))
            {
                return ApiResponseFactory.Failure<object>(
                        message: "User login failed",
                        errorType: "Password_Missing",
                        errorMessage: "Password is required",
                        statusCode: StatusCodes.Status400BadRequest
                );
            }

            var data = new User
            {
                Email = request.Email,
                PasswordHash = request.Password
            };
            var result = await _loginRepository.Login(data);

            if(result == null)
            { 
                return ApiResponseFactory.Failure<object>(
                        message: "User login failed",
                        errorType: "User_Not_Found",
                        errorMessage: "No account with this eamil",
                        statusCode: StatusCodes.Status401Unauthorized
                );
            }

            if (result.IsActive == false)
            {
                return ApiResponseFactory.Failure<object>(
                        message: "User login failed",
                        errorType: "Account_Inactive",
                        errorMessage: "User account is inactive",
                        statusCode: StatusCodes.Status403Forbidden
                );
            }
            if (result.EmailConfirmed == false)
            {
                return ApiResponseFactory.Failure<object>(
                        message: "User login failed",
                        errorType: "Email_Not_Verified",
                        errorMessage: "User email is not verified",
                        statusCode: StatusCodes.Status403Forbidden
                );
            }
            var user = new User
            {
                Id = result.Id, 
                Email = result.Email,
                PasswordHash = result.PasswordHash
            };
            
            string requestPasswordHash;

            try
            {
                var decodedvalue = Encoding.UTF8.GetString(Convert.FromBase64String(request.Password));
                var json = JsonDocument.Parse("{" + decodedvalue + "}");
                requestPasswordHash = json.RootElement.GetProperty("hashedPassword").GetString();
            }
            catch
            {
                requestPasswordHash = null;
            }
            if (requestPasswordHash != user.PasswordHash)
            {
                return ApiResponseFactory.Failure<object>(
                        message: "User login failed",
                        errorType: "Invalid_Credentials",
                        errorMessage: "Incorrect password",
                        statusCode: StatusCodes.Status401Unauthorized
                );
            }
            else
            {
                return ApiResponseFactory.Success<object>(
                        data: new { Token = GenerateToken(user),
                        result.Id,
                        result.FirstName,
                        result.LastName,
                        result.Email},
                        message: "User login successful",
                        statusCode: StatusCodes.Status200OK
                );
            }
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
