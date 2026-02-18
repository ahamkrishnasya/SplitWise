using SplitWise.Application.DTOs.Sessions;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace SplitWise.Application.Services
{
    public class LoginService: ILoginService
    {
        private readonly ILoginRepository _loginRepository; 
        private readonly IConfiguration _configuration; 

        public LoginService(ILoginRepository loginRepository, IConfiguration Configuration)
        {
            _loginRepository = loginRepository;
            _configuration = Configuration;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto request)
        {
            var data = new User
            {
                Email = request.Email,
                PasswordHash = request.Password
            };
            var result = await _loginRepository.Login(data);

            if(result == null || result.IsActive == false)
            { 
                return null; 
            } 

            var user = new User
            {
                Id = result.Id, 
                Email = result.Email,
                PasswordHash = result.PasswordHash
            };
            
            string requestPasswordHash;
            var decodedvalue = Encoding.UTF8.GetString(Convert.FromBase64String(request.Password));
            var json = JsonDocument.Parse("{" + decodedvalue + "}");
            requestPasswordHash = json.RootElement.GetProperty("hashedPassword").GetString();

            if (requestPasswordHash == user.PasswordHash)
            {
                return new LoginResponseDto { Token = GenerateToken(user) };
            }
            return null;
        }

        private string GenerateToken(User user)
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
