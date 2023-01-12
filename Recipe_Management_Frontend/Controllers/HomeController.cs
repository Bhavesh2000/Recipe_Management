using Microsoft.AspNetCore.Mvc;
using Recipe_Management_Frontend.Models;
using System.Diagnostics;

namespace Recipe_Management_Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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