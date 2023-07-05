using AccountingService.Controllers.models;
using System.Security.Claims;

namespace AccountingService.Services
{
    public class ClaimsService
    {
        public List<Claim> GetUserClaims(SystemUser systemUser)
        {
            var claims =  new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, systemUser.Email));
            return claims;
        }
    }
}
