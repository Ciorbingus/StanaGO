using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StanaGO.Data;
using StanaGO.Models;
using StanaGO.ViewModels;


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

            var farms = await _context.Sheepfarms.Include(f => f.Products) .Where(f => f.OwnerId == userId).ToListAsync();

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
        [AllowAnonymous]
        public async Task<IActionResult> Sheepfarm(int id)
        {
            var farm = await _context.Sheepfarms.Include(f => f.Products) .FirstOrDefaultAsync(m => m.Id == id);

            if (farm == null) return NotFound();

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
                return RedirectToAction(nameof(Sheepfarm), new { id = product.FarmId });
            }
            return View(product);
        }

        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> Product(int id)
        {
            var product = await _context.Products.Include(p => p.Farm).ThenInclude(f => f.Owner).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Shepherd")]
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var currentUserId = _userManager.GetUserId(User);

            var product = await _context.Products.Include(p => p.Farm).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            if (product.Farm.OwnerId != currentUserId)
            {
                return Unauthorized(); 
            }

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Status = product.Status,
                CurrentImagePath = product.ImagePath
            };

            return View(model);
        }

        [Authorize(Roles = "Shepherd")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var currentUserId = _userManager.GetUserId(User);

            var product = await _context.Products
                                        .Include(p => p.Farm)
                                        .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (product == null) return NotFound();

            if (product.Farm.OwnerId != currentUserId) return Unauthorized();

            product.Name = model.Name;
            product.Price = model.Price;
            product.Description = model.Description;
            product.Status = model.Status;

            if (model.NewImage != null)
            {
                string folderPath = Path.Combine(_hostEnvironment.WebRootPath, "images", "products");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    string oldPath = Path.Combine(folderPath, product.ImagePath);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.NewImage.FileName;
                string filePath = Path.Combine(folderPath, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NewImage.CopyToAsync(fileStream);
                }

                product.ImagePath = uniqueFileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Product", new { id = product.Id });
        }


        [Authorize(Roles = "Shepherd")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var farm = await _context.Sheepfarms.FirstOrDefaultAsync(f => f.Id == id);

            if (farm == null) return NotFound();
            if (farm.OwnerId != currentUserId) return Unauthorized();

            var model = new SheepFarmViewModel
            {
                Id = farm.Id,
                Name = farm.Name,
                Address = farm.Address,
                Latitude = farm.Latitude,
                Longitude = farm.Longitude
            };

            return View("EditSheepfarm", model); 
        }

        [Authorize(Roles = "Shepherd")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SheepFarmViewModel model)
        {
            if (!ModelState.IsValid) return View("EditSheepfarm", model);

            var currentUserId = _userManager.GetUserId(User);
            var farm = await _context.Sheepfarms.FirstOrDefaultAsync(f => f.Id == model.Id);

            if (farm == null) return NotFound();
            if (farm.OwnerId != currentUserId) return Unauthorized();

            farm.Name = model.Name;
            farm.Address = model.Address;
            farm.Latitude = model.Latitude;
            farm.Longitude = model.Longitude;

            await _context.SaveChangesAsync();

            return RedirectToAction("Sheepfarm", new { id = farm.Id });
        }

    }
}