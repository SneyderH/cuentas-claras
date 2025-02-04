using System.Diagnostics;
using System.Threading.Tasks;
using Cuentas_Claras_Client.Connection;
using Cuentas_Claras_Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cuentas_Claras_Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiController _apiController;
        private readonly ConnectionDB _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApiController apiController, ConnectionDB context)
        {
            _apiController = apiController;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var apiResponse = await _apiController.GetTokenAsync(username, password);

            if (apiResponse != null && apiResponse.Success)
            {
                var user = new Users
                {
                    Username = username,
                    Password = apiResponse.Token
                };

                _context.users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Error al registrar usuario";
            return View();
        }

        public IActionResult Index()
        {
            return View();
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
