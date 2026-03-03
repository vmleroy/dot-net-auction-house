using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuctionHouse.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace AuctionHouse.Application.Services
{
    public class JwtTokenGenerator
    {
        private readonly string _jwtSecret;
        private readonly string _issuer;
        private readonly string _audience;
        public JwtTokenGenerator(string jwtSecret, string issuer, string audience)
        {
            _jwtSecret = jwtSecret;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateToken(User user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Id))
                throw new InvalidOperationException("User must have an email, username, and ID to generate a token.");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
