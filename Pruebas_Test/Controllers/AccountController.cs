using Microsoft.AspNetCore.Mvc;
using Pruebas_Test.Models;

namespace Pruebas_Test.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
    sfgsgfgsdg    public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validación de credenciales de prueba solicitadas: admin / 123
            if (model.Usuario == "admin" && model.Password == "123456")
            {
                return Redirect("https://www.google.com");
            }

            ModelState.AddModelError(
                string.Empty,
                "Usuario o contraseña incorrectos."
            );

            return View(model);
        }
    }
}