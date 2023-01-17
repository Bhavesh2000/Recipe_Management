using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Recipe_Management_Frontend.Models;
using System.Diagnostics;
using System.Collections.Generic;
using NuGet.Common;
using Newtonsoft.Json.Linq;

namespace Recipe_Management_Frontend.Controllers
{
   struct Token
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
        public bool result { get; set; }
    }
    public class HomeController : Controller
    {
       
        public async Task<string> RefreshToken(string token,string refreshToken)
        {
            try
            {

                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7082/api/");
                Token t = new Token();
                t.token = token.ToString();
                t.refreshToken=refreshToken.ToString();
                HttpContent body = new StringContent(JsonConvert.SerializeObject(new {token=token,refreshToken=refreshToken}), System.Text.Encoding.UTF8, "application/json");

                var response = client.PostAsync("account/RefreshToken",body).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content =await response.Content.ReadAsStringAsync();

                    return content;
                }
              
            }catch(Exception ex)
            {
                return "Something went wrong!!!";
            }
            return "Something went wrong!!!";



        }

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
                        var responseTask = client.GetAsync("Recipe/GetAcceptedRecipes");
                        responseTask.Wait();

                        var result = responseTask.Result;
                        var readTask = await result.Content.ReadAsStringAsync();
                        
                        if (result.IsSuccessStatusCode)
                        {
                            recipes = JsonConvert.DeserializeObject<IList<Recipe>>(readTask);
                            if (TempData["message"]!=null)
                            {
                                TempData["message"] = TempData["message"];
                                TempData["type"] = TempData["type"];
                            }
                            return View(recipes);
                        }
                        else
                        {
                            Console.WriteLine("Login to view Recipes");
                            TempData["message"] = "Login to view Recipes";
                            TempData["type"] = "error";
                            return RedirectToAction("LogOut", "Auth");
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
            
            return RedirectToAction("LogOut", "Auth");
        }


        [HttpPost]
        public async Task<ActionResult> AddRecipe(string recipeName,string category,string ingredients,string cookingProcess)
        {
            try
            {
                string userId = Request.Cookies["userId"];
                string token = Request.Cookies["token"];
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7082/api/");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpContent body = new StringContent(JsonConvert.SerializeObject(new { name = recipeName, ingredients = ingredients, procedure = cookingProcess, userId = userId, category = category }), System.Text.Encoding.UTF8, "application/json");

                var response = client.PostAsync("Recipe/AddRecipe", body).Result;

                    var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(content);
                    if (!string.IsNullOrEmpty(content))
                    {
                        Console.WriteLine("Recipe Saved Successfully.");
                        TempData["message"] = "Recipe added successfully";
                        TempData["type"] = "success";
                        return RedirectToAction("Index");
                    }
                    

                }
                    TempData["message"] =content;
                    TempData["type"] = "error";
                    return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["message"] = "Something went wrong!";
                TempData["type"] = "error";
            }
            
            return RedirectToAction("LogOut","Auth");
        }

        [HttpPut]
        public async Task<IActionResult> EditRecipe(string recipeName, string category, string ingredients, string cookingProcess)
        {
            try
            {
                string userId = Request.Cookies["userId"];
                string token = Request.Cookies["token"];
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7082/api/");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpContent body = new StringContent(JsonConvert.SerializeObject(new { name = recipeName, ingredients = ingredients, procedure = cookingProcess, userId = userId, category = category }), System.Text.Encoding.UTF8, "application/json");

                var response = client.PutAsync("Recipe/EditRecipe", body).Result;

                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(content);
                    if (!string.IsNullOrEmpty(content))
                    {
                        Console.WriteLine("Recipe Edited Successfully.");
                        TempData["message"] = "Recipe Edited successfully";
                        TempData["type"] = "success";
                        return RedirectToAction("GetRecipesByUser");
                    }


                }
                TempData["message"] = content;
                TempData["type"] = "error";
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["message"] = "Something went wrong!";
                TempData["type"] = "error";
            }

            return RedirectToAction("LogOut", "Auth");
        }

        [HttpDelete]
        public IActionResult DeleteRecipe(int id)
        {
                string userId = Request.Cookies["userId"];
                string token = Request.Cookies["token"];
                try
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("https://localhost:7082/api/");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    var response = client.DeleteAsync("recipe/deleterecipe?id=" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync();

                    TempData["message"] = "Recipe deleted successfully";
                    TempData["type"] = "success";
                    return RedirectToAction("GetRecipesByUser");
                     }
                    TempData["message"] = "Something went wrong!!!";
                    TempData["type"] = "error";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["message"] = ex.Message == "UserName is not provided" ? "Login to view Recipes" : "Failed to fetch recipe";
                    TempData["type"] = "error";

                }

   
            return RedirectToAction("LogOut", "Auth");
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
                            if(recipe.Status == "Accepted")
                            {
                                return View(recipe); 
                            }
                        }
                        TempData["message"] = "Failed to fetch recipe";
                        TempData["type"] = "error";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["message"] = "Failed to fetch recipe";
                    TempData["type"] = "error";
                }
            }
            return RedirectToAction("LogOut","Auth");
        }

        [Route("myrecipes")]
        public async Task<ActionResult> GetRecipesByUser()
        {
            string token = Request.Cookies["token"];
            string userId = Request.Cookies["userId"];
            IEnumerable<Recipe> recipes = null;

            if (token != null)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://localhost:7082/api/");
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                        var responseTask = client.GetAsync("Recipe/GetRecipeByUserId?UserId="+userId);
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
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["message"] = ex.Message == "UserName is not provided" ? "Login to view Recipes" : "Failed to fetch recipe";
                    TempData["type"] = "error";
                }
            }
            return RedirectToAction("LogOut","Auth");
        }


        [Route("/pending-requests/{id}")]
        public async Task<IActionResult> displaybyId(int id)
        {
            string type = Request.Cookies["type"];

            string token = Request.Cookies["token"];
            string refreshtoken = Request.Cookies["refreshtoken"];
            if (token != null && type=="Admin")
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
                                    return View("PendingRequestById", recipe);
                                }
                                TempData["message"] = "Failed to fetch recipe";
                                TempData["type"] = "error";
                                return RedirectToAction("Index");
                            }
                        
                    }
                
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["message"] = "Something went wrong!!!";
                    TempData["type"] = "error";
                    
                }
            }
            return RedirectToAction("LogOut", "Auth");
        }

        [Route("/pending-requests")]
        public async Task<IActionResult> PendingRequest()
        {
            string type = Request.Cookies["type"];

            string token = Request.Cookies["token"];
            string refreshtoken = Request.Cookies["refreshtoken"];

            if (token != null && type=="Admin")
            {


                try
                {
                    
                        var client = new HttpClient();
                        client.BaseAddress = new Uri("https://localhost:7082/api/");
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                        var response = client.GetAsync("recipe/getpendingrecipes").Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();

                            List<Recipe> recipes = JsonConvert.DeserializeObject<List<Recipe>>(content);
                            return View("PendingRequest", recipes);
                        }
                        TempData["message"] = "Failed to fetch pending requests";
                        TempData["type"] = "error";
                        return RedirectToAction("Index");
                    
                  
                }
                catch (Exception ex)
                {
                    TempData["message"] = ex.Message == "UserName is not provided" ? "Login to view Recipes" : "Failed to fetch recipe";
                    TempData["type"] = "error";
                }
               
            }
            return RedirectToAction("LogOut", "Auth");



        }
        
        

        public IActionResult Accept(int id)
        {
            string token = Request.Cookies["token"];
            string type = Request.Cookies["type"];

            if (token != null && type == "Admin")
            {
                try
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("https://localhost:7082/api/");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                   var response = client.PutAsync("recipe/Update_Status_Accept_Recipe?id="+id,null).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content =response.Content.ReadAsStringAsync();

                        return RedirectToAction("PendingRequest");
                    }
                    TempData["message"] = "Something went wrong!!!";
                    TempData["type"] = "error";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["message"] = ex.Message == "UserName is not provided" ? "Login to view Recipes" : "Failed to fetch recipe";
                    TempData["type"] = "error";
                }

            }
            return RedirectToAction("LogOut", "Auth");


        }



        public IActionResult Delete(int id)
        {
            string token = Request.Cookies["token"];
            string type = Request.Cookies["type"];

            if (token != null && type == "Admin")
            {
                try
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("https://localhost:7082/api/");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    var response = client.PutAsync("recipe/Update_Status_Reject_Recipe?id=" + id,null).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content =response.Content.ReadAsStringAsync();

                        return RedirectToAction("PendingRequest");
                    }
                    TempData["message"] = "Something went wrong!!!";
                    TempData["type"] = "error";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["message"] = ex.Message == "UserName is not provided" ? "Login to view Recipes" : "Failed to fetch recipe";
                    TempData["type"] = "error";
                 
                }

            }
            return RedirectToAction("LogOut", "Auth");


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