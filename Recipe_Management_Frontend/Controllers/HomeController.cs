using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Recipe_Management_Frontend.Models;
using System.Diagnostics;
using System.Collections.Generic;
using NuGet.Common;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Net;

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


        public async Task<HttpResponseMessage> Apifunction(string route,string method,HttpContent body,int id =0)
        {
            string token = Request.Cookies["token"];

            if (token != null)
            {
                try
                {

                    using (var client = new HttpClient())
                    {

                        client.BaseAddress = new Uri("https://localhost:7082/api/Recipe/");
                        if (route != "Search")
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                        }
                        if (method == "GET")
                        {

                            if (id != 0)
                            {
                                var response = client.GetAsync(String.Format("{0}?id={1}", route, id)).Result;
                                return response;
                            }
                            else
                            {
                                var response = client.GetAsync(route).Result;
                                return response;
                            }

                        }

                        if (method == "POST")
                        {

                            var response = client.PostAsync(route, body).Result;
                            return response;
                        }
                        if (method == "PUT")
                        {
                            if (body != null)
                            {
                                var response = client.PutAsync(route, body).Result;
                                return response;
                            }
                            else
                            {
                                var response = client.PutAsync(String.Format("{0}?id={1}", route, id), null).Result;
                                return response;
                            }

                        }
                        if (method == "DELETE")
                        {
                            var response = client.DeleteAsync(String.Format("{0}?id={1}", route, id)).Result;
                            return response;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
            }
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
          }

        public async Task<ActionResult> Index()
        {
            IEnumerable<Recipe> recipes = null;
           

           
                try
                {
                    
                        

                        var response = await Apifunction("GetAcceptedRecipes", "GET",null);
                        var content = await response.Content.ReadAsStringAsync();
                        
                        if (response.IsSuccessStatusCode)
                        {
                            recipes = JsonConvert.DeserializeObject<IList<Recipe>>(content);
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
                    
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["message"] = "Failed to fetch recipe";
                    TempData["type"] = "error";
                }
            return RedirectToAction("LogOut", "Auth");
        }
            
           
        


        [HttpPost]
        public async Task<ActionResult> AddRecipe(string recipeName,string category,string ingredientsList, string cookingProcess)
        {
        
            try
            {
            
                
                StringContent body = new StringContent(JsonConvert.SerializeObject(new { name = recipeName, ingredients = ingredientsList, procedure = cookingProcess, category = category }), System.Text.Encoding.UTF8, "application/json");

                var response = Apifunction("AddRecipe","POST", body).Result;

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


        public async Task<IActionResult> EditRecipe(int id,string recipeName, string category, string ingredientsList, string cookingProcess)
        {
            try
            {
                
                HttpContent body = new StringContent(JsonConvert.SerializeObject(new {Id=id, name = recipeName, ingredients = ingredientsList, procedure = cookingProcess, category = category}), System.Text.Encoding.UTF8, "application/json");

                var response = Apifunction("UpdateRecipe","PUT", body).Result;

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

        
        public IActionResult DeleteRecipe(int id)
        {
              
                try
                {
                    

                    var response =Apifunction("deleterecipe","DELETE",null, id).Result;

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
          

            
                try
                {
                  
                        var response = Apifunction("GetRecipeById","GET",null, id);



                        var content = await response.Result.Content.ReadAsStringAsync();
                        if (response.Result.IsSuccessStatusCode)
                        {
                            var recipe = JsonConvert.DeserializeObject<Recipe>(content);
                            if(recipe.Status == "Accepted")
                            {
                                return View(recipe); 
                            }
                        }
                        TempData["message"] = "Failed to fetch recipe";
                        TempData["type"] = "error";
                        return RedirectToAction("Index");
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["message"] = "Failed to fetch recipe";
                    TempData["type"] = "error";
                }
            return RedirectToAction("LogOut", "Auth");
        }
            
        

        [Route("myrecipes")]
        public async Task<ActionResult> GetRecipesByUser()
        {
          
            IEnumerable<Recipe> recipes = null;

            
                try
                {
                    
                        var response = await Apifunction("GetRecipeByUserId","GET",null);
                        

                      
                        var content = await response.Content.ReadAsStringAsync();
                        
                        if (response.IsSuccessStatusCode)
                        {
                            recipes = JsonConvert.DeserializeObject<IList<Recipe>>(content);
                            return View(recipes);
                        }
                        TempData["message"] = "Failed to fetch recipe";
                        TempData["type"] = "error";
                        return RedirectToAction("Index");
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["message"] = ex.Message == "UserName is not provided" ? "Login to view Recipes" : "Failed to fetch recipe";
                    TempData["type"] = "error";
                }
            return RedirectToAction("LogOut", "Auth");
        }
            
        


        [Route("/pending-requests/{id}")]
        public async Task<IActionResult> displaybyId(int id)
        {
            string type = Request.Cookies["type"];

          
            if ( type=="Admin")
            {
                try
                {
                                 
                               
                                var response= await Apifunction("GetRecipeById","GET",null, id);
                                var readTask = await response.Content.ReadAsStringAsync();
                                if (response.IsSuccessStatusCode)
                                {
                                    var recipe = JsonConvert.DeserializeObject<Recipe>(readTask);
                                    return View("PendingRequestById", recipe);
                                }
                                TempData["message"] = "Failed to fetch recipe";
                                TempData["type"] = "error";
                                return RedirectToAction("Index");
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

        

            if ( type=="Admin")
            {


                try
                {
                    

                        var response = Apifunction("getpendingrecipes","GET",null).Result;
                        

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
            string type = Request.Cookies["type"];

            if (type == "Admin")
            {
                try
                {
                    
                   var response = Apifunction("Update_Status_Accept_Recipe","PUT",null,id).Result;

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
            string type = Request.Cookies["type"];

            if ( type == "Admin")
            {
                try
                {
                    

                    var response = Apifunction("Update_Status_Reject_Recipe","PUT",null,id).Result;

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


        public async Task<IActionResult> Search(string search)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7082/api/");
                var response =client.GetAsync("recipe/search?search=" + search).Result;
                Console.WriteLine(response.StatusCode.ToString());
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    
                    List<Recipe> recipes = JsonConvert.DeserializeObject<List<Recipe>>(content);
                    return View("Index", recipes);

                }
                else if(content=="Type to Search")
                {
                    TempData["message"] = content;
                    TempData["type"] = "error";
                    return RedirectToAction("Index");
                }
               
                else if(response.StatusCode.ToString()=="NotFound")
                {
                    TempData["info"] = "No such recipes or username found!!!";
                    
                }

            }
            catch (Exception ex)
            {

                TempData["message"] = ex.Message;
                TempData["type"] = "error";
            }
            return View("Index");
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