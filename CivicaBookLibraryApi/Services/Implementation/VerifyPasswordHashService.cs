using CivicaBookLibraryApi.Models;
using CivicaBookLibraryApi.Services.Contract;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CivicaBookLibraryApi.Services.Implementation
{
    [ExcludeFromCodeCoverage]
    public class VerifyPasswordHashService : IVerifyPasswordHashService
    {
        private readonly IConfiguration _configuration;


        public VerifyPasswordHashService(IConfiguration configuration)
        {
            _configuration = configuration; //this is registering the dependency. it is call dependency injection

        }
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }


        public string CreateToken(User user)
        {
            List<Claim> claimes = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                new Claim(ClaimTypes.Name,user.LoginId.ToString()),
               new Claim("UserId", user.UserId.ToString()),
               new Claim("Admin", user.IsAdmin.ToString()),

            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimes),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = signingCredentials

            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
