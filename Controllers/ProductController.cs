using Microsoft.AspNetCore.Mvc;

namespace StanaGO.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index ( )
        {
            return View ();
        }
    }
}
