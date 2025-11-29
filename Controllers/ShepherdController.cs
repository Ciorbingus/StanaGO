using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StanaGO.Data;
using StanaGO.Models;


namespace StanaGO.Controllers
{
    [Authorize(Roles = "Shepherd")] 
    public class ShepherdController : Controller
    {
        private readonly StanaGOContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ShepherdController(StanaGOContext context, UserManager<User> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> YourFarms()
        {
            var userId = _userManager.GetUserId(User);

            var farms = await _context.Sheepfarms
                              .Include(f => f.Products) 
                              .Where(f => f.OwnerId == userId)
                              .ToListAsync();

            return View(farms);
        }

        [HttpGet]
        public IActionResult RegisterFarm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sheepfarm farm)
        {
            var userId = _userManager.GetUserId(User);
            farm.OwnerId = userId;

            ModelState.Remove("Owner");
            ModelState.Remove("OwnerId");

            if (ModelState.IsValid)
            {
                _context.Sheepfarms.Add(farm);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(YourFarms));
            }

            return View(farm);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var farm = await _context.Sheepfarms
                .Include(f => f.Products) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (farm == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (farm.OwnerId != userId) return Forbid();

            return View(farm);
        }

        
        [HttpGet]
        public IActionResult AddProduct(int farmId)
        {
            return View(new Product { FarmId = farmId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product)
        {
            ModelState.Remove("Farm");
            ModelState.Remove("ImagePath");

            if (ModelState.IsValid)
            {
                product.Price = Math.Abs(product.Price);

                if (product.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
                    string extension = Path.GetExtension(product.ImageFile.FileName);

                    product.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                    string path = Path.Combine(wwwRootPath + "/images/products/");

                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    string filePath = Path.Combine(path, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    product.ImagePath = "default.png";
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = product.FarmId });
            }
            return View(product);
        }

    }
}