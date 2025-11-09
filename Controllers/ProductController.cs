using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StanaGO.Data;

namespace StanaGO.Controllers
{
    public class ProductController : Controller
    {
        private readonly StanaGOContext _context;

        public ProductController ( StanaGOContext context )
        {
            _context = context;
        }

        public IActionResult Index ( )
        {
            return View ();
        }
    }
}
