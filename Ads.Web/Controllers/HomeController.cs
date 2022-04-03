using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ads.Data;
using Ads.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ads.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Ads;Integrated Security=true;";
        public IActionResult Index()
        {
            var repo = new AdsRepository(_connectionString);
            int? userId = GetCurrentUserId();
            List<Ad> ads = repo.GetAllAds();
            HomeViewModel vm = new HomeViewModel
            {
                Ads = ads.Select(ad => new AdViewModel
                {
                    Ad = ad,
                    CanDelete = userId != null && ad.UserId == userId
                }).ToList()
            };
            return View(vm);
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var repo = new AdsRepository(_connectionString);
            var isAuthenticted = User.Identity.IsAuthenticated;
            if (!isAuthenticted)
            {
                return Redirect("/account/login");
            }
            var user = repo.GetByEmail(User.Identity.Name);
            ad.UserId = user.Id;
            repo.NewAd(ad);
            return Redirect("/");
        }
        [HttpPost]
        public IActionResult DeleteAd (int adId)
        {
            var repo = new AdsRepository(_connectionString);
            repo.DeleteAd(adId);
            return Redirect("/");
        }
        [Authorize]
        public  IActionResult MyAccount()
        {
            var repo = new AdsRepository(_connectionString);
            int? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Redirect("/");
            }
            
            List<Ad> usersAds = repo.GetAdsForId(userId);
            return View(usersAds);
        }
        private int? GetCurrentUserId()
        {
            var repo = new AdsRepository (_connectionString);
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }
            var user = repo.GetByEmail(User.Identity.Name);
            if (user == null)
            {
                return null;
            }

            return user.Id;
        }

    }
}
