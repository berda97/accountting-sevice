using AccountingService.Controllers;
using AccountingService.Controllers.models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace AccountingService.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Token GetToken(List<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]));

            var expiryInMinutes = Convert.ToInt32(15);


            var token = new JwtSecurityToken(

                claims: claims,
               issuer: _configuration["JwtConfig:Issuer"],
                audience: _configuration["JwtConfig:Audience"],
                expires: DateTime.Now.AddMinutes(expiryInMinutes),
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return new Token
            {
                Value = new JwtSecurityTokenHandler().WriteToken(token),
                ValidTo = token.ValidTo,

            };
        }
    }
}
