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

        public async Task<object> VerifyAndGenerateToken(TokenDto tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;


                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);
                //Verifing Jwt token received
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                    StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return null;
                    }
                }

                var jwtExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x =>
                                                        x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = TimeStampToDateTime(jwtExpiryDate);

                if (expiryDate > DateTime.Now)
                {
                    return new
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Expired token"
                        }
                    };
                }
                //getting stored token from database
                var storedToken = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedToken == null)
                {
                    return new
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Invalid Tokens"
                        }
                    };
                }

                if (storedToken.IsUsed)
                {
                    return new
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Invalid Tokens"
                        }
                    };
                }

                if (storedToken.IsRevoked)
                {
                    return new
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Invalid Tokens"
                        }
                    };
                }

                //getting JWT token Id
                var jti = tokenInVerification.Claims.FirstOrDefault(x =>
                                       x.Type == JwtRegisteredClaimNames.Jti).Value;
                //Verifing if jwt id and stored id is same
                if (storedToken.JwtId != jti)
                {
                    return new
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Invalid Token"
                        }
                    };
                }
                //if refresh token expired then returning false result
                if (storedToken.ExpireDate < DateTime.Now)
                {
                    return new
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Expired Token"
                        }
                    };
                }

                storedToken.IsUsed = true;
                _db.RefreshTokens.Update(storedToken);
                await _db.SaveChangesAsync();

                var dbUser = await _db.Users.FindAsync(storedToken.UserId);

                return await JwtTokenGenerator(dbUser);

            }
            catch (Exception)
            {
                return new
                {
                    Result = false,
                    Errors = new List<string>()
                        {
                            "Server Error"
                        }
                };
            }
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

            //var refreshToken = new RefreshToken()
            //{
            //    JwtId = token.Id,     //Giving same Id as JWT token Id
            //    Token = RandomStringGenerator(23),  //Actual RefreshToken generated
            //    AddedDate = DateTime.Now,     //Refresh token Generated time
            //    ExpireDate = DateTime.Now.AddHours(1),  //Expiry of refresh token
            //    UserId = user.Id,   //User info for whom we generated this token
            //    IsRevoked = false,
            //    IsUsed = false,
            //};

         //   await _db.RefreshTokens.AddAsync(refreshToken);  //Added refresh token to database
          //  await _db.SaveChangesAsync();



            var result = new
            {
                Token = jwtToken,
              //  RefreshToken = refreshToken.Token,
            };

            return result;

        }

        public async Task<string> VerifyJwtAndGetUserId(string jwt)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var tokenInVerification = jwtTokenHandler.ValidateToken(jwt, _tokenValidationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                StringComparison.InvariantCultureIgnoreCase);

                if (result == false)
                {
                    return null;
                }
            }

            string userId = validatedToken.Id.ToString();

            return userId;
        }


        public async Task DeleteAllRefreshTokenForUser(string userId)
        {
            var refreshTokens = await _db.RefreshTokens.Where(t => t.UserId == userId).ToListAsync();
            _db.RefreshTokens.RemoveRange(refreshTokens);
            await _db.SaveChangesAsync();

        }


        private DateTime TimeStampToDateTime(long unixDateTime)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            dateTimeVal = dateTimeVal.AddSeconds(unixDateTime).ToLocalTime();

            return dateTimeVal;
        }


        //To create actual Refresh Token
        private string RandomStringGenerator(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
