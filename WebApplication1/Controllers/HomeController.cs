using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["GUID"] = await GenerateGuidAsync();
            return View();
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate()
        {
            var guid = await GenerateGuidAsync();
            return Ok(guid);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private Task<string> GenerateGuidAsync()
        {
            var guid = Guid.NewGuid().ToString();
            return Task.FromResult(guid);
        }
    }
}