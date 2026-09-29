using Microsoft.AspNetCore.Mvc;

namespace Pruebas_Test.Controllers
{
    public class PruebasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
