//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Recipe_Management_System.AppDbContext;
//using Recipe_Management_System.Models;

//namespace Recipe_Management_System.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {
//        private readonly ApplicationDbContext context;
//        public UserController(ApplicationDbContext _context)
//        {
//            context = _context;
//        }

//        [HttpPost]
//        public async Task<ActionResult<User>> AddUser(User user)
//        {
            
//            try
//            {
//                context.Users.Add(user);
//                await context.SaveChangesAsync();
//            }
//            catch (DbUpdateException)
//            {
//                return Conflict();
//            }
//            return Ok(user);
//        }
//    }
//}
