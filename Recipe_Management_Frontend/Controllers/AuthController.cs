using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recipe_Management_Frontend.Models;
using System.Text.RegularExpressions;

namespace Recipe_Management_Frontend.Controllers
{
    struct userToken
    {
        public string token { get; set;}
        public string refreshtoken { get; set;}
        public bool result { get; set;}
    }
    struct cu
    {
        public userToken UserToken;
        public bool result;
        public string type;
        public string message;
        public string stautus;
        public List<string> errors;
    }

   
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
        public async Task<IActionResult> RegisterOne(Register r)
        {
            

            if (!ModelState.IsValid)
            {
                return View("Register", r);
            }
            bool y = CheckPasswordStregth(r.Password);
            if (!y)
            {
                TempData["errors"] = "Minimum Password Length should be 8 and password should be Alphanumeric";
                return View("Register", r);
            }



            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5162");
            HttpContent body = new StringContent(JsonConvert.SerializeObject(new { Name = r.Name, Email = r.Email, Password = r.Password }), System.Text.Encoding.UTF8, "application/json");
            var response = client.PostAsync("/api/account/Register", body).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(content))
                {
                    var objDeserializeObject = JsonConvert.DeserializeObject<cu>(content);



                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Today.AddDays(1);
                    Response.Cookies.Append("token", objDeserializeObject.UserToken.token, options);
                    Response.Cookies.Append("type", "user");
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    var objDeserializeObject = JsonConvert.DeserializeObject<cu>(content);
                    TempData["errors"] = objDeserializeObject.errors[0];
                    TempData["type"] = "error";


                    return View("Register", r);

                }

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
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5162");
            HttpContent body = new StringContent(JsonConvert.SerializeObject(new { Email = email, Password = password }), System.Text.Encoding.UTF8, "application/json");
            var response = client.PostAsync("/api/account/login", body).Result;


            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    var objDeserializeObject = JsonConvert.DeserializeObject<cu>(content);



                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Today.AddDays(1);
                    userToken u = objDeserializeObject.UserToken;
                    Response.Cookies.Append("token", u.token, options);
                    Response.Cookies.Append("type", "user");

                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    var objDeserializeObject = JsonConvert.DeserializeObject<cu>(content);
                    if (objDeserializeObject.errors != null)
                    {
                        TempData["errors"] = objDeserializeObject.errors[0];
                    }
                    else
                    {
                        TempData["errors"] = "Invalid Email";
                    }
                    TempData["type"] = "error";
                }
                return View("Index");

            }
            return View("Index");
        }

        public IActionResult LogOut()
        {
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("type");
            return RedirectToAction("Index");
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
