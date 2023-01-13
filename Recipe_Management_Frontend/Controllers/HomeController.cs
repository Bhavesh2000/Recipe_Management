using Microsoft.AspNetCore.Mvc;
using Recipe_Management_Frontend.Models;
using System.Diagnostics;

namespace Recipe_Management_Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        HttpClient client;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            client = new HttpClient();
        }

        public IActionResult Index()
        {
            string token = Request.Cookies["token"];
            var response = new HttpResponseMessage();
            if (token != null)
            {

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }

        }


        [HttpPost]
        public IActionResult AddRecipe(string recipeName,string category,string ingredients,string cookingProcess)
        {
            Console.WriteLine(recipeName);

            return RedirectToAction("Index");
        }

        [Route("recipe")]
        public IActionResult GetRecipeById(int id)
        {
            Console.WriteLine(id);
            return View();
        }

        [Route("myrecipes")]
        public IActionResult GetRecipesByUser()
        {
            Console.WriteLine("getUser");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}