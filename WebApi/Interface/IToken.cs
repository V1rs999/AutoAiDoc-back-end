using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApi.Interface
{
    public interface IToken
    {
        List<Claim> GetClaimsForJwt();
        JwtSecurityToken GetToken(List<Claim> authClaims);
        bool isExpired(string token);
    }
}
