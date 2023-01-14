using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recipe_Management_Frontend.Models;
using System.Diagnostics;

namespace Recipe_Management_Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        HttpClient client;
        List<Recipe> r = new List<Recipe>()
            {
                new Recipe() {id=1,name="Biryani",Username="grishma",Procedure="cgjkm",Ingredients="xcvbn",Category="nonveg"}
            };

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5162");

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
            Console.WriteLine("getUser");
            return View();
        }
        [Route("/pending-requests/{id}")]
        public IActionResult displaybyId(int id)
        {
            Recipe p = r.Find(x => x.id == id);
            return View("PendingRequestById",p);
        }
        [Route("/pending-requests")]
        public async Task<IActionResult> PendingRequest()
        {
           
            //var response = client.GetAsync("/api/recipe/getpendingrecipes").Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var content = await response.Content.ReadAsStringAsync();

            //    List<Recipe> recipes=JsonConvert.DeserializeObject<List<Recipe>>(content);
            //    return View("PendingRequest",recipes);
            //}
            //else
            //{
            //    return View();
            //}
            return View("PendingRequest", r);

          
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