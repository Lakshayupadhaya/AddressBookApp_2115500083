using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Helper
{
    public class Jwt
    {
        private readonly IConfiguration _config;
        public Jwt(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string GenerateToken(UserEntity user) //After marking it as static you are unable to access the _config so used dependency injection and not static method
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserId", user.UserId.ToString()),
                new Claim("Firstname", user.FirstName),
                new Claim("Lastname", user.LastName),
                //new Claim("UserName", UserName),
                new Claim("Email", user.Email),
                //new Claim("Phone", Phone)
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
    };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpirationMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
