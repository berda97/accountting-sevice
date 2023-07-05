using AccountingService.Controllers.models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountingService.Services
{
    public class JwtTokenService
    {
        public Token  GetToken(List<Claim> claims)
        {
          
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("IiwibmFtZSI6IkpvaG4gRG94cThIIoDvwdueQB468K5xDc5633seEF"));
            var expiryInMinutes = Convert.ToInt32(15);

            var token = new JwtSecurityToken(
              
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryInMinutes),
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new Token
            {
                Value = new JwtSecurityTokenHandler().WriteToken(token),
                ValidTo = token.ValidTo
            };
        }
    }
}
