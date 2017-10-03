using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GamingWebApp.Models;
using GamingWebApp.Proxies;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace GamingWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptionsSnapshot<WebAppSettings> settings;
        private readonly ILogger logger;

        public HomeController(IOptionsSnapshot<WebAppSettings> settings) //, ILogger logger)
        {
            //this.logger = logger;
            this.settings = settings;
        }
        public async Task<IActionResult> Index()
        {
            LeaderboardProxy proxy = new LeaderboardProxy(settings.Value.LeaderboardWebApiBaseUrl, logger);
            var leaderboard = await proxy.GetLeaderboardAsync();
            return View(leaderboard);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
