using Microsoft.AspNetCore.Identity;
using Recipe_Management_System.Models;
using Recipe_Management_System.Models.Dto;

namespace Recipe_Management_System.Repository.Service
{
    public interface ITokenGenerator
    {
        Task<object> JwtTokenGenerator(User user);

        Task DeleteAllRefreshTokenForUser(string userId);
    }
}
