using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Recipe_Management_Frontend.Models;

namespace Recipe_Management_Frontend.Controllers
{
    
    public class AuthController : Controller
    {
        // GET: AuthController

        [Route("/LogIn")]
        public ActionResult Index()
        {
            return View();
        }


        [Route("/Register")]
        public IActionResult Register()
        {
            return View();
        }

      
        [HttpPost]
        public IActionResult RegisterOne(Register r)
        {
            if (!ModelState.IsValid)
            {
                return View("Register",r);
            }

            return Content("Hello");
            
          

        }

        [Route("/admin")]
        public IActionResult Admin()
        {
            return View("AdminLogIn");
        }


        

       

    }
}
