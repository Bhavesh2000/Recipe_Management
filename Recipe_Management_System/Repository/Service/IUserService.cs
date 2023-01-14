using Recipe_Management_System.Models;

namespace Recipe_Management_System.Repository.Service
{
    public interface IUserService
    {
        IEnumerable<User> GetAllAsync();

        Task<User> GetUser(string id);
    }
}
