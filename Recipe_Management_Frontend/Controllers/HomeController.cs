using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Recipe_Management_Frontend.Models;
using System.Diagnostics;
using System.Collections.Generic;
using NuGet.Common;

namespace Recipe_Management_Frontend.Controllers
{
    public class HomeController : Controller
    {
        List<Recipe> r = new List<Recipe>()
            {
                new Recipe() {Id=1,Name="Biryani",Username="grishma",Procedure="cgjkm",Ingredients="xcvbn",Category="nonveg"}
            };

        public async Task<ActionResult> Index()
        {
            IEnumerable<Recipe> recipes = null;
            string token = Request.Cookies["token"];

            if (token != null)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://localhost:7082/api/");
                        client.DefaultRequestHeaders.Add("Authorization","Bearer "+ token);
                        var responseTask = client.GetAsync("Recipe/GetAllRecipes");
                        responseTask.Wait();

                        var result = responseTask.Result;
                        var readTask = await result.Content.ReadAsStringAsync();
                        
                        if (result.IsSuccessStatusCode)
                        {
                            recipes = JsonConvert.DeserializeObject<IList<Recipe>>(readTask);
                            return View(recipes);
                        }
                        else
                        {
                            Console.WriteLine("Login to view Recipes");
                            TempData["message"] = "Login to view Recipes";
                            TempData["type"] = "error";
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["message"] = "Failed to fetch recipe";
                    TempData["type"] = "error";
                }
            }
            
            return RedirectToAction("Index", "Auth");
        }


        [HttpPost]
        public async Task<ActionResult> AddRecipe(string recipeName,string category,string ingredients,string cookingProcess)
        {
            try
            {
                string username = Request.Cookies["username"];
                string token = Request.Cookies["token"];
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7082/api/");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpContent body = new StringContent(JsonConvert.SerializeObject(new { name = recipeName, ingredients = ingredients, procedure = cookingProcess, username = username, category = category }), System.Text.Encoding.UTF8, "application/json");
       
                var response = client.PostAsync("Recipe/AddRecipe", body).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                    if (!string.IsNullOrEmpty(content))
                    {
                        Console.WriteLine("Recipe Saved Successfully.");
                        TempData["message"] = "Recipe added successfully";
                        TempData["type"] = "success";
                        return RedirectToAction("Index");
                    }
                }
                TempData["message"] = "Failed to add recipe";
                TempData["type"] = "error";
            }
            catch(Exception ex) { 
                Console.WriteLine(ex.Message);
                TempData["message"] = "Failed to add recipe";
                TempData["type"] = "error";
            }
            
            return RedirectToAction("Index");
        }

        [Route("recipe")]
        public async Task<ActionResult> GetRecipeById(int id)
        {
            string token = Request.Cookies["token"];

            if (token != null)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://localhost:7082/api/");
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                        var responseTask = client.GetAsync("Recipe/GetRecipeById?id=" + id);
                        responseTask.Wait();

                        var result = responseTask.Result;
                        var readTask = await result.Content.ReadAsStringAsync();
                        if (result.IsSuccessStatusCode)
                        {
                            var recipe = JsonConvert.DeserializeObject<Recipe>(readTask);
                            return View(recipe); 
                        }
                        TempData["message"] = "Failed to fetch recipe";
                        TempData["type"] = "error";
                        
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["message"] = "Failed to fetch recipe";
                    TempData["type"] = "error";
                }
            }
            return RedirectToAction("Index");
        }

        [Route("myrecipes")]
        public async Task<ActionResult> GetRecipesByUser()
        {
            string token = Request.Cookies["token"];
            string username = Request.Cookies["username"];
            IEnumerable<Recipe> recipes = null;

            if (token != null)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://localhost:7082/api/");
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                        var responseTask = client.GetAsync("Recipe/GetRecipeByUserName?Username="+username);
                        responseTask.Wait();

                        var result = responseTask.Result;
                        var readTask = await result.Content.ReadAsStringAsync();
                        Console.WriteLine(readTask);
                        if (result.IsSuccessStatusCode)
                        {
                            recipes = JsonConvert.DeserializeObject<IList<Recipe>>(readTask);
                            return View(recipes);
                        }
                        TempData["message"] = "Failed to fetch recipe";
                        TempData["type"] = "error";

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["message"] = ex.Message == "UserName is not provided" ? "Login to view Recipes" : "Failed to fetch recipe";
                    TempData["type"] = "error";
                }
            }
            return RedirectToAction("Index");
        }
        [Route("/pending-requests/{id}")]
        public IActionResult displaybyId(int id)
        {
            Recipe p = r.Find(x => x.Id == id);
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