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
using Recipe_Management_System.Repository.Service;
using Microsoft.AspNetCore.Authorization;

namespace Recipe_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUserService _userService;

        public AccountController(UserManager<IdentityUser> userManager,
            IUserService userService,
            ITokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _userService = userService;
            _tokenGenerator = tokenGenerator;
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
                    UserName = request.Email,
                    Name = request.Name,
                    Type = request.Type,
                };

                var is_created = await _userManager.CreateAsync(new_user, request.Password);
                if (!is_created.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                                    new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
                }

                var user = await _userManager.FindByEmailAsync(new_user.Email);

                var jwtToken = await _tokenGenerator.JwtTokenGenerator(new_user);
                return Ok(new
                {
                    UserToken= jwtToken,
                    Type = new_user.Type,
                    User_Id = user.Id,
                    Message = "User is Added successfully",
                    Result = true
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
                var user = await _userService.GetUser(existing_user.Id);

                var jwtToken =await _tokenGenerator.JwtTokenGenerator(existing_user);

                return Ok(new
                {
                    UserToken = jwtToken,
                    Type = user.Type,
                    User_Id = user.Id,
                    Result = true
                   // UserId = existing_user.Id
                });
            }

            return BadRequest(new
            {
                Message = "Invalid Payload"
            });
        }

        [HttpDelete]
        [Route("LogoutJWT")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<object> Logout2()
        {

            //var userId = id;
            var userId = User.FindFirstValue("id");

            if (userId == null)
            {
                return Unauthorized(new
                {
                    Result = false,

                });
            }

         //   await _tokenGenerator.DeleteAllRefreshTokenForUser(userId);

            return Ok(new
            {
                Massage = "Logout Sucessfully"
            });
        }

        [HttpPost]
        [Route("RefreshToken")]
      //  [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> RefreshToken(TokenDto tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await _tokenGenerator.VerifyAndGenerateToken(tokenRequest);

                if (result == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(result);
                }
            }

            return BadRequest();
        }



    }
}
