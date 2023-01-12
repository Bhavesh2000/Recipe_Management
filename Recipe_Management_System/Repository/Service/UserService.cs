using Recipe_Management_System.AppDbContext;
using Recipe_Management_System.Models;
using Recipe_Management_System.Repository.Base;

namespace Recipe_Management_System.Repository.Service
{
    public class UserService : EntityBaseRepository<User>, IUserService
    {
        public UserService(ApplicationDbContext context) : base(context)
        {

        }
    }
}
