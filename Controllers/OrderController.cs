using Microsoft.AspNetCore.Mvc;

namespace StanaGO.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
