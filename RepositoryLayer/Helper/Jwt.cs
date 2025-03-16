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
using RepositoryLayer.Interface;

namespace RepositoryLayer.Helper
{
    public class Jwt
    {
        private readonly IAddressBookRL _addressBookRL;
        private readonly IConfiguration _config;
        public Jwt(IAddressBookRL addressBookRL, IConfiguration configuration) 
        {
            _addressBookRL = addressBookRL;
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
                new Claim("Role", user.Role)
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

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true; // Token is valid
            }
            catch
            {
                return false; // Token is invalid or expired
            }
        }

        public (bool Autharised, bool found) ValidateToken(string token, int id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var userIdClaim = principal.FindFirst("UserId");
                int userId = int.Parse(userIdClaim.Value);

                (bool authorised, bool found) = _addressBookRL.AuthoriseAndFindRL(userId, id);

                if(principal.FindFirstValue("Role") == "Admin")
                {
                    return (true, found);
                }
                return (authorised, found);
                //if (matched)
                //{
                //    return (true, true); // Token is valid and Updated id matches user id
                //}
                //return (false, true);
            }
            catch
            {
                return (false, false); // Token is invalid or expired
            }
        }

        public ClaimsPrincipal? GetClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal; // Returns ClaimsPrincipal with extracted claims
            }
            catch
            {
                return null; // Return null if token is invalid
            }
        }
        public int GetUserIdFromClaims(ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst("UserId");
            return int.Parse(userIdClaim.Value);
        }

        public (string role, int userId)? GetRoleAndUserId(string token) 
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var roleClaim = principal.FindFirst("Role");
                string role = roleClaim.Value;
                var userIdClaim = principal.FindFirst("UserId");
                int userId = int.Parse(roleClaim.Value);

                return (role, userId);
            }
            catch
            {
                return null; // Return null if token is invalid
            }
        }
    }
}
