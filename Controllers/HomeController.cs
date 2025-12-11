using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StanaGO.Data;
using StanaGO.Models;
using System.Diagnostics;

namespace StanaGO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StanaGOContext _context;

        public HomeController(ILogger<HomeController> logger, StanaGOContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetNearbyData(double lat, double lng)
        {
            var allFarms = await _context.Sheepfarms
                                         .Include(f => f.Products)
                                         .ToListAsync();

            var nearbyProducts = new List<object>();

            foreach (var farm in allFarms)
            {
                double distance = CalculateDistance(lat, lng, farm.Latitude, farm.Longitude);

                if (distance <= 30000)
                {
                    foreach (var prod in farm.Products)
                    {
                        nearbyProducts.Add(new
                        {
                            Id = prod.Id,
                            Name = prod.Name,
                            Price = prod.Price,
                            Image = prod.ImagePath,
                            FarmName = farm.Name,
                            Distance = (distance / 1000).ToString("0.0") + " km" 
                        });
                    }
                }
            }

            return Json(new
            {
                farms = allFarms.Select(f => new { f.Name, f.Latitude, f.Longitude, f.Address }),
                products = nearbyProducts
            });
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3;
            var radLat1 = lat1 * Math.PI / 180;
            var radLat2 = lat2 * Math.PI / 180;
            var deltaLat = (lat2 - lat1) * Math.PI / 180;
            var deltaLon = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(radLat1) * Math.Cos(radLat2) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; 
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Welcome ( )
        {
            if ( User.Identity.IsAuthenticated )
            {
                return RedirectToAction ("Index", "Home");
            }
            return View ();     
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
