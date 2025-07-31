using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ForgeHub.JwtToken
{
    public class JwtTokenGenerator
    {

        IConfiguration config;

        public JwtTokenGenerator(IConfiguration config)
        {
            this.config = config;
        }

        public string GenerateToken(int userId, string email, string fullName, string role)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, fullName),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role ?? "User")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
