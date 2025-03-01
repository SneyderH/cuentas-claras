using Cuentas_Claras_Client.Connection;
using Cuentas_Claras_Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cuentas_Claras_Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiController _apiController;
        private readonly ConnectionDB _context;
        private readonly IConfiguration _configuration;
        //private readonly ILogger<HomeController> _logger;


        public HomeController(ApiController apiController, ConnectionDB context, IConfiguration configuration)
        {
            _apiController = apiController;
            _context = context;
            _configuration = configuration;
            //_logger = logger;
        }

        #region LOGIN
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View("~/Views/Home/Login/Login.cshtml");
        }

        [HttpPost]
        [AllowAnonymous]
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

                var tokenTask = _apiController.TokenLogin(request.Username, request.Password);
                tokenTask.Wait();

                var token = tokenTask.Result;
                var principal = ValidateTokenAndCreatePrincipal(token);

                if (principal == null)
                {
                    return BadRequest("Token no válido");
                }

                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddDays(1),
                    IsPersistent = true
                };

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties).Wait();

                return RedirectToAction("Principal", "Home");
            } 
            catch (Exception ex)
            {
                //Cambiar la alerta de usuario no encontrado *********************************
                Console.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("Login", "Home");
            }
        }
        #endregion


        private ClaimsPrincipal ValidateTokenAndCreatePrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                if (!tokenHandler.CanReadToken(token))
                {
                    Console.WriteLine("El token no es un JWT válido");
                    return null;
                }

                var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt:Key").Value!);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar el token: {ex.Message}");
                return null;
            }
        }

        #region REGISTER
            [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Home/Login/Register.cshtml");
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


        [Authorize]
        public IActionResult Principal()
        {
            //Console.WriteLine("Usuario autenticado: " + User.Identity.Name);
            return View("~/Views/Home/Content/Principal.cshtml");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction("Index", "Home");
        }

        //public IActionResult AccessDenied()
        //{
        //    return View();
        //}

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
