using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AQCricket_ERP.Common.Utilities
{
    public static class GenerateToken
    {
        public static string Authenticate(ApplicationUsers user, string secretToken, string issuer)
        {
            string token = string.Empty;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretToken);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                     //   new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                       // new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        //new Claim("Id", user.Id),
                        new Claim(JwtRegisteredClaimNames.Aud, issuer),
                        new Claim(JwtRegisteredClaimNames.Iss, issuer)
                    }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var genToken = tokenHandler.CreateToken(tokenDescriptor);
            token = tokenHandler.WriteToken(genToken);
            return token;
        }
    }
}
