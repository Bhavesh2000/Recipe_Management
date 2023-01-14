using Microsoft.AspNetCore.Identity;
using Recipe_Management_System.Models.Dto;

namespace Recipe_Management_System.Repository.Service
{
    public interface ITokenGenerator
    {
        Task<object> VerifyAndGenerateToken(TokenDto tokenRequest);

        Task<object> JwtTokenGenerator(IdentityUser user);

        Task DeleteAllRefreshTokenForUser(string userId);
    }
}
