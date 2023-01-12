using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Recipe_Management_System.Models.Dto;
using Recipe_Management_System.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Recipe_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly JwtConfig _jwtConfig;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            //_jwtConfig = jwtConfig;
            _configuration = configuration;
        }



        //We can add this attribute on top of our http methods to provide authorization.
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {

            //Validate the request
            if (ModelState.IsValid)
            {

                //Check if email already exists.
                var user_exist = await _userManager.FindByEmailAsync(request.Email);

                if (user_exist != null)
                {
                    return BadRequest(new
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Email already exist"
                        }
                    });
                }


                //create user
                var new_user = new User()
                {
                    Email = request.Email,
                    UserName = request.Name,
                    PhoneNumber = request.PhoneNum
                };

                var is_created = await _userManager.CreateAsync(new_user, request.Password);

                if (is_created.Succeeded)
                {
                    var token = JwtTokenGenerator(new_user);

                    return Ok(new
                    {
                        Token = token,
                        Result = true
                    });
                }

                return BadRequest(new
                {
                    Errors = new List<string>()
                        {
                            "Server Error"
                        }
                });


            }

            return BadRequest();
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginRequest)
        {
            if (ModelState.IsValid)
            {
                //check if user exists
                var existing_user = await _userManager.FindByEmailAsync(loginRequest.Email);

                if (existing_user == null)
                {
                    return BadRequest(new
                    {
                        Result = false,

                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existing_user, loginRequest.Password);

                if (!isCorrect)
                {
                    return BadRequest(new
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Invalid Credentials"
                        }
                    });
                }

                var jwtToken = JwtTokenGenerator(existing_user);

                return Ok(new
                {
                    Token = jwtToken,
                    Result = true
                });
            }

            return BadRequest(new
            {
                Message = "Invalid Payload"
            });
        }

        private string JwtTokenGenerator(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }


        private string CheckPasswordStregth(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8 && (Regex.IsMatch(password, "[a-z]")) && (Regex.IsMatch(password, "[A-Z]"))
                    && (Regex.IsMatch(password, "[0-9]")) )
            {
                sb.Append("Minimum Password Length should be 8 and password should be Aplhanumeric " + Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
