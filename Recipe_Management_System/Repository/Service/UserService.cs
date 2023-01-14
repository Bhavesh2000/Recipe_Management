using Recipe_Management_System.AppDbContext;
using Recipe_Management_System.Models;

namespace Recipe_Management_System.Repository.Service
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _Context;
        public UserService(ApplicationDbContext Context)
        {
            _Context = Context;
        }
        public IEnumerable<User> GetAllAsync()
        {
            var actors = _Context.Set<User>().ToList();
            return actors;
        }

        public async Task<User> GetUser(string id)
        {
            return await _Context.Users.FindAsync(id);
        }

    }

}
