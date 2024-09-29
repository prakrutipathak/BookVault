using System.IdentityModel.Tokens.Jwt;

namespace CivicaBookLibraryMVC.Infrastructure
{
    public interface IJwtTokenHandler
    {
        JwtSecurityToken ReadJwtToken(string token);

    }
}
