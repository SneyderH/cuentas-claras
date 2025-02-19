using System.Diagnostics;
using System.Threading.Tasks;
using Cuentas_Claras_Client.Connection;
using Cuentas_Claras_Client.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Cuentas_Claras_Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiController _apiController;
        private readonly ConnectionDB _context;
        //private readonly ILogger<HomeController> _logger;

        public HomeController(ApiController apiController, ConnectionDB context)
        {
            _apiController = apiController;
            _context = context;
            //_logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Users request)
        {
            var user = _context.users.SingleOrDefault(u => u.Username == request.Username);

            try
            {
                if (user == null)
                {
                    return BadRequest("Usuario no encontrado");
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    return BadRequest("Contraseña incorrecta");
                }

                //var tokenTask = _apiController.TokenLoginAsync(request.Username, request.Password);

                //tokenTask.Wait();
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return RedirectToAction("Login", "Home");
        }

        #region REGISTER
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var apiResponse = await _apiController.GetTokenAsync(username, password);

            if (apiResponse != null)
            {
                var user = new Users
                {
                    Username = username,
                    Password = apiResponse.Password
                };

                _context.users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login", "Home");
            }
            ViewBag.Error = "Error al registrar usuario";
            return View();
        }
        #endregion

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
