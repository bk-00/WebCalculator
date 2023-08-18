using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebCalculator.Models;

namespace WebCalculator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new HomeViewModel());
        }

        [HttpPost]
        public IActionResult Index(HomeViewModel homeViewModel)
        {
            string strtmpMsg = string.Empty;
            double dblRet = 0;
            dblRet = clsCompute.calculate(homeViewModel.Expression, out strtmpMsg);
            homeViewModel.Ans = dblRet;
            homeViewModel.Message = strtmpMsg;

            if (string.IsNullOrEmpty(strtmpMsg))
                homeViewModel.ValidExpression = true;
            else
                homeViewModel.ValidExpression = false;

            return View(homeViewModel);
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