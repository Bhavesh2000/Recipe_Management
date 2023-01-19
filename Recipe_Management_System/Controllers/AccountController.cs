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
using System.Linq;

namespace Recipe_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        ResponseDto _response;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUserService _userService;

        public AccountController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserService userService,
            ITokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _userService = userService;
            _tokenGenerator = tokenGenerator;
            _roleManager = roleManager;
            _response = new ResponseDto();

        }



        //We can add this attribute on top of our http methods to provide authorization.
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        [HttpPost]
        [Route("Register")]
        public async Task<object> Register([FromBody] RegisterDto request)
        {
            if (!_roleManager.RoleExistsAsync(Helper.Helper.Admin).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(Helper.Helper.Admin));
                await _roleManager.CreateAsync(new IdentityRole(Helper.Helper.User));
            }
            try
            {
                if (ModelState.IsValid)
                {
                    if(request.Name.Contains(' ')) 
                    {
                        _response.IsSuccess = false;
                        _response.Result = BadRequest();
                        _response.DisplayMessage = "Name should not have spaces";
                    }

                    var IsUserNamePresent = await _userManager.FindByNameAsync(request.Name);
                    var IsEmailPresent = await _userManager.FindByEmailAsync(request.Email);
                    if (IsUserNamePresent != null)
                    {
                        _response.IsSuccess = false;
                        _response.Result = BadRequest();
                        _response.DisplayMessage = "UserName Already Present";
                    }
                    else if (IsEmailPresent != null)
                    {
                        _response.IsSuccess = false;    
                        _response.Result = BadRequest();
                        _response.DisplayMessage = "Email Already Present";
                    }
                    else
                    {
                        var new_user = new User()
                        {
                            Email = request.Email,
                            UserName = request.Name,
                            Type = "User",
                        };
                        var result = await _userManager.CreateAsync(new_user, request.Password);

                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(new_user, "User");

                            var user = await _userManager.FindByEmailAsync(new_user.Email);

                            var jwtToken = await _tokenGenerator.JwtTokenGenerator(new_user);

                            _response.Result = new
                            {
                                UserToken = jwtToken,
                                Type = new_user.Type,
                                User_Id = user.Id,
                            };
                        }
                    }

                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Result = BadRequest();
                    _response.DisplayMessage = "Invalid request";
                    //_response.ErrorMessages = new List<string>() { ex.ToString() };
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;

        }


        [HttpPost]
        [Route("Login")]
        public async Task<object> Login([FromBody] LoginDto loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    _response.Result = BadRequest();
                    _response.DisplayMessage = "Invalid model state";
                    return _response;
                }
                else
                {
                    var existing_user = await _userManager.FindByEmailAsync(loginRequest.Email);
                    if (existing_user == null)
                    {
                        _response.IsSuccess = false;
                        _response.Result = Unauthorized();
                        _response.DisplayMessage = "User Does not Exist";
                        return _response;
                    }
                    var isCorrect = await _userManager.CheckPasswordAsync(existing_user, loginRequest.Password);
                    if (!isCorrect)
                    {
                        _response.IsSuccess = false;
                        _response.Result = Unauthorized();
                        _response.DisplayMessage = "Password Incorrect";
                        return _response;
                    }

                    var user = await _userService.GetUser(existing_user.Id);

                    var jwtToken = await _tokenGenerator.JwtTokenGenerator(user);

                    _response.Result = new
                    {
                        UserToken = jwtToken,
                        Type = user.Type,
                        User_name = user.UserName,
                        User_Id = user.Id,
                        Result = true
                        // UserId = existing_user.Id
                    };
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            _response.DisplayMessage = "Logged In successfully";
            return (_response);

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

            await _tokenGenerator.DeleteAllRefreshTokenForUser(userId);

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


        [HttpPost]
        [Route("RegisterAdmin")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<object> RegisterAdmin([FromBody] RegisterDto request)
        {
            if (!_roleManager.RoleExistsAsync(Helper.Helper.Admin).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(Helper.Helper.Admin));
                await _roleManager.CreateAsync(new IdentityRole(Helper.Helper.User));
            }

            try
            {
                if (ModelState.IsValid)
                {

                    var IsUserNamePresent = await _userManager.FindByNameAsync(request.Name);
                    var IsEmailPresent = await _userManager.FindByEmailAsync(request.Email);
                    if (IsUserNamePresent != null)
                    {
                        _response.Result = BadRequest();
                        _response.DisplayMessage = "UserName Already Present";
                    }
                    else if (IsEmailPresent != null)
                    {
                        _response.Result = BadRequest();
                        _response.DisplayMessage = "Email Already Present";
                    }
                    else
                    {
                        var new_user = new User()
                        {
                            Email = request.Email,
                            UserName = request.Name,
                            Type = "Admin",
                        };
                        var result = await _userManager.CreateAsync(new_user, request.Password);

                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(new_user, "Admin");

                            var user = await _userManager.FindByEmailAsync(new_user.Email);

                            var jwtToken = await _tokenGenerator.JwtTokenGenerator(new_user);

                            _response.Result = new
                            {
                                UserToken = jwtToken,
                                Type = new_user.Type,
                                User_Id = user.Id,
                            };
                        }
                    }

                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Result = BadRequest();
                    _response.DisplayMessage = "Invalid request";
                    //_response.ErrorMessages = new List<string>() { ex.ToString() };
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

    }
}
