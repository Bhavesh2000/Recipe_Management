using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recipe_Management_Frontend.Models;
using System.Text.RegularExpressions;

namespace Recipe_Management_Frontend.Controllers
{
    public struct userToken
    {
        public string token { get; set; }
    }
    struct result
    {
        public userToken UserToken;
        public string user_Id;
        public string type;
        public string statusCode;
        public string user_name;
    }


    struct cu
    {
        public result Result;
        public bool isSuccess;
        public string displayMessage;
        
        public List<string> errorMessages;
    }

   
    public class AuthController : Controller
    {
        // GET: AuthController

        [Route("/LogIn")]
        public ActionResult Index()
        {
            if (Request.Cookies["token"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        [Route("/Register")]
        public IActionResult Register()
        {
            if (Request.Cookies["token"] != null)
            {
                return RedirectToAction("Index","Home");
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RegisterOne(Register r)
        {


            if (!ModelState.IsValid)
            {
                return View("Register", r);
            }
            bool y = CheckPasswordStregth(r.Password);
            if (!y)
            {
                TempData["type"] = "error";
                TempData["message"] = "Minimum Password Length should be 8 and password should be combination of uppercase, lowercase, special characters and numbers";
                return View("Register", r);
            }


            try
            {

                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7082");
                HttpContent body = new StringContent(JsonConvert.SerializeObject(new { Name = r.Name, Email = r.Email, Password = r.Password }), System.Text.Encoding.UTF8, "application/json");
                var response = client.PostAsync("/api/account/Register", body).Result;
                var content = await response.Content.ReadAsStringAsync();
                var objDeserializeObject = JsonConvert.DeserializeObject<cu>(content);

                if (objDeserializeObject.Result.user_Id != null)
                {
  
                        CookieOptions options = new CookieOptions();
                        
                        Response.Cookies.Append("token", objDeserializeObject.Result.UserToken.token);

                        Response.Cookies.Append("type", objDeserializeObject.Result.type);
                    Response.Cookies.Append("userName", r.Name);

                     //   Response.Cookies.Append("userId", objDeserializeObject.Result.user_Id);
                        //  Response.Cookies.Append("refreshtoken", objDeserializeObject.UserToken.refreshtoken);
                        TempData["message"] = "User registered successfully!!!";
                        TempData["type"] = "success";
                        return RedirectToAction("Index", "Home");
                    }
                
                else
                {
                  
                    
                        TempData["type"] = "error";
                      
                        if (objDeserializeObject.errorMessages != null)
                        {
                            TempData["message"] = objDeserializeObject.errorMessages[0];
                        }
                        else if (objDeserializeObject.displayMessage != null)
                        {
                            TempData["message"] = objDeserializeObject.displayMessage;
                        }

                        return View("Register", r);

                    }

            }
            catch (Exception ex)
            {
                TempData["message"] = "Something went wrong";
                TempData["type"] = "error";
            }
            return View("Register", r);
        }
           

        




        [Route("/admin")]
        public IActionResult Admin()
        {
            return View("AdminLogIn");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7082");
                HttpContent body = new StringContent(JsonConvert.SerializeObject(new { Email = email, Password = password }), System.Text.Encoding.UTF8, "application/json");
                var response = client.PostAsync("/api/account/login", body).Result;
                var content = await response.Content.ReadAsStringAsync();
                var objDeserializeObject = JsonConvert.DeserializeObject<cu>(content);

                if (objDeserializeObject.Result.user_Id != null)
                {

                        CookieOptions options = new CookieOptions();
                        
                        Response.Cookies.Append("token",objDeserializeObject.Result.UserToken.token);

                        Response.Cookies.Append("type", objDeserializeObject.Result.type);
                    Response.Cookies.Append("userName", objDeserializeObject.Result.user_name);
                        //      Response.Cookies.Append("refreshtoken", u.refreshtoken);
                      //  Response.Cookies.Append("userId", objDeserializeObject.Result.user_Id);
                        TempData["message"] = "Logged in successfully!!!";
                        TempData["type"] = "success";
                        return RedirectToAction("Index", "Home");
                  }
                
                else
                {
                    
                        TempData["type"] = "error";

                        if (objDeserializeObject.errorMessages != null)
                        {
                            TempData["message"] = objDeserializeObject.errorMessages[0];
                        }
                        else
                        {
                            TempData["message"] = objDeserializeObject.displayMessage;
                        }

                    }


                
            }
            catch (Exception ex)
            {
                TempData["message"] = "Something went wrong";
                TempData["type"] = "error";
            }


            return View("Index");

        }

        public IActionResult LogOut()
        {
            try
            {
                string token = Request.Cookies["token"];

                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7082/api/");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var response = client.DeleteAsync("account/LogoutJWT").Result;

               
                if (response.IsSuccessStatusCode)
                {

                    if (TempData["message"] != null)
                    {
                        TempData["message"] = TempData["message"];
                        TempData["type"] = TempData["type"];
                    }
                    else
                    {
                        TempData["message"] = "Logged out successfully!!!";
                        TempData["type"] = "success";
                    }
                }
                Response.Cookies.Delete("token");
                //            Response.Cookies.Delete("refreshtoken");

                Response.Cookies.Delete("type");
          //      Response.Cookies.Delete("userId");


            }
            catch (Exception ex)
            {
                TempData["message"] = "Something went wrong";
                TempData["type"] = "error";
            }

            return RedirectToAction("Index", "Auth");
        }

        [Route("/{id}")]
        public IActionResult Error(string id)
        {
            if (Request.Cookies["token"] != null)
            {
                TempData["message"] = "Page not found!!";
                TempData["type"] = "error";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["message"] = "Please Login!!";
                TempData["type"] = "error";
                return RedirectToAction("Index");
            }
            
           

        }

        bool CheckPasswordStregth(string password)
        {
            if (password.Length < 8)
            {

                return false;
            }
            else if (!(Regex.IsMatch(password, "[a-z]")))
            {
                return false;
            }
            else if (!Regex.IsMatch(password, "[A-Z]"))
            {
                return false;
            }
            else if (!Regex.IsMatch(password, "[0-9]"))
            {
                return false;
            }
            else if (!Regex.IsMatch(password, "[@#$%*!&^]"))
            {
                return false;
            }
            return true;
        }


    }
}
