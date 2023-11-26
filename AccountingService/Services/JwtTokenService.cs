using AccountingService.Controllers;
using AccountingService.Controllers.models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Text;
using AccountingService.Interface;

namespace AccountingService.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ITimeService _timeService;
        public JwtTokenService(IConfiguration configuration, ITimeService timeService)
        {
            _configuration = configuration;
            _timeService = timeService;
        }
        public Token GetToken(List<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]));
            var expiryInMinutes = int.Parse(_configuration["JwtConfig:ExpiryInMinutes"]);
            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["JwtConfig:Issuer"],
                audience: _configuration["JwtConfig:Audience"],
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            if (_timeService.Now > token.ValidTo)
            {
                throw new SecurityTokenExpiredException("Token has already expired.");
            }

            return new Token
            {
                Value = new JwtSecurityTokenHandler().WriteToken(token),
                ValidTo = token.ValidTo,
            };
        }
    }
}

