using Microsoft.AspNetCore.Mvc;

namespace Cuentas_Claras_Client.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApiController _apiController;

        public RegisterController(ApiController apiController)
        {
            _apiController = apiController;
        }

        public async Task<IActionResult> Register(string username, string password)
        {
            var token = await _apiController.GetTokenAsync(username, password);

            if (token != null)
            {
                HttpContext.Session.SetString("Token", token);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Error al registrar usuario";
            return View();
        }
    }
}
