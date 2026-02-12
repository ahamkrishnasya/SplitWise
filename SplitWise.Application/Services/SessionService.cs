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
    public class SessionService: ISessionService
    {
        private readonly ISessionRepository _sessionRepository; 
        private readonly IConfiguration _configuration; 

        public SessionService(ISessionRepository SessionRepository, IConfiguration Configuration)
        {
            _sessionRepository = SessionRepository;
            _configuration = Configuration;
        }

        public async Task<SessionResponseDto> Login(SessionRequestDto request)
        {
            var data = new User
            {
                Email = request.Email,
                PasswordHash = request.Password
            };
            await _sessionRepository.Login(data);

            var user = new User
            {
                Email = data.Email,
                PasswordHash = data.PasswordHash
            };
            string requestPasswordHash;
            if (user != null)
            {
                var decodedvalue = Encoding.UTF8.GetString(Convert.FromBase64String(request.Password));
                var json = JsonDocument.Parse("{" + decodedvalue + "}");
                requestPasswordHash = json.RootElement.GetProperty("hashedPassword").GetString();
            }

            return new SessionResponseDto { Token = GenerateToken(user) };
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
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
