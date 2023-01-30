using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Recipe_Management_System.Models.Dto;
using Recipe_Management_System.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Recipe_Management_System.AppDbContext;
using Microsoft.EntityFrameworkCore;

namespace Recipe_Management_System.Repository.Service
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly UserManager<IdentityUser> _userManager;

        public TokenGenerator(IConfiguration configuration, 
            ApplicationDbContext db, 
            TokenValidationParameters tokenValidationParameters,
            UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _db = db;
            _tokenValidationParameters = tokenValidationParameters;
            _userManager = userManager;
        }
        public async Task<object> JwtTokenGenerator(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                    new Claim(ClaimTypes.Role , user.Type.ToString()),
                    
                }),
                Expires = DateTime.Now.Add(TimeSpan.Parse(_configuration.GetSection("JwtConfig:ExpiryTimeFrame").Value)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            var result = new
            {
                Token = jwtToken
            };

            return result;

        }

        public async Task DeleteAllRefreshTokenForUser(string userId)
        {
            var refreshTokens = await _db.RefreshTokens.Where(t => t.UserId == userId).ToListAsync();
            _db.RefreshTokens.RemoveRange(refreshTokens);
            await _db.SaveChangesAsync();

        }


    }
}
