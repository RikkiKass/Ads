using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Ads.Data;
using Ads.Web.Models;

namespace Ads.Web.Controllers
{
    public class AccountController : Controller
    {
      

        private readonly string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Ads;Integrated Security=true;";
        
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Signup(User user, string password)
        {
            AdsRepository repo = new AdsRepository(_connectionString);
            repo.AddUser(user, password);
            return Redirect("/");
        }
        public IActionResult Login()
        {
          
            return View(new LoginViewModel { Message = (string)TempData["message"] });
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {

            AdsRepository repo = new AdsRepository(_connectionString);
            User user=repo.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid Login";
                return RedirectToAction("Login");
            }
            else
            {
                var claims = new List<Claim>
            {
               
                new Claim("user", email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();
                return Redirect("/");
            }
        

           
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }

    }
}
