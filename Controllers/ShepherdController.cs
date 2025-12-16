using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StanaGO.Data;
using StanaGO.Enums;
using StanaGO.Models;
using StanaGO.ViewModels;
using System.Threading;


namespace StanaGO.Controllers
{    public class ShepherdController : Controller
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

            var farms = await _context.Sheepfarms.Include(f => f.Products).Where(f => f.OwnerId == userId).ToListAsync();

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
            var farm = await _context.Sheepfarms.Include(f => f.Products).FirstOrDefaultAsync(m => m.Id == id);

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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isModerator = User.IsInRole("Moderator");

            var product = await _context.Products.Include(p => p.Farm).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            if (product.Farm.OwnerId != currentUserId && !isModerator)
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

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var currentUserId = _userManager.GetUserId(User);
            var isModerator = User.IsInRole("Moderator");

            var product = await _context.Products.Include(p => p.Farm).FirstOrDefaultAsync(p => p.Id == model.Id);

            if (product == null) return NotFound();

            if (product.Farm.OwnerId != currentUserId && !isModerator) return Unauthorized();

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


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isModerator = User.IsInRole("Moderator");

            var farm = await _context.Sheepfarms.FirstOrDefaultAsync(f => f.Id == id);

            if (farm == null) return NotFound();
            if (farm.OwnerId != currentUserId && !isModerator) return Unauthorized();

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

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SheepFarmViewModel model)
        {
            if (!ModelState.IsValid) return View("EditSheepfarm", model);

            var currentUserId = _userManager.GetUserId(User);
            var isModerator = User.IsInRole("Moderator");

            var farm = await _context.Sheepfarms.FirstOrDefaultAsync(f => f.Id == model.Id);

            if (farm == null) return NotFound();
            if (farm.OwnerId != currentUserId && !isModerator) return Unauthorized();

            farm.Name = model.Name;
            farm.Address = model.Address;
            farm.Latitude = model.Latitude;
            farm.Longitude = model.Longitude;

            await _context.SaveChangesAsync();

            return RedirectToAction("Sheepfarm", new { id = farm.Id });
        }

        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            var currentUserId = _userManager.GetUserId(User);
            var isModerator = User.IsInRole("Moderator");

            var product = await _context.Products.Include(p => p.Farm).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            if (product.Farm.OwnerId != currentUserId && !isModerator)
            {
                return Forbid();
            }

            return View(product);
        }

        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous] 
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            var currentUserId = _userManager.GetUserId(User);
            var isModerator = User.IsInRole("Moderator");

            var product = await _context.Products.Include(p => p.Farm).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            if (product.Farm.OwnerId != currentUserId && !isModerator)
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                string filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", "products", product.ImagePath);
                if (System.IO.File.Exists(filePath))
                {
                    try { System.IO.File.Delete(filePath); } catch { }
                }
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Sheepfarm", new { id = product.FarmId });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteFarm(int id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            var currentUserId = _userManager.GetUserId(User);
            var isModerator = User.IsInRole("Moderator");

            var farm = await _context.Sheepfarms.Include(f => f.Owner).Include(f => f.Products).FirstOrDefaultAsync(m => m.Id == id);

            if (farm == null) return NotFound();

            if (farm.OwnerId != currentUserId && !isModerator)
            {
                return Forbid();
            }

            return View(farm);
        }

        [HttpPost, ActionName("DeleteFarm")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            var currentUserId = _userManager.GetUserId(User);
            var isModerator = User.IsInRole("Moderator");

            var farm = await _context.Sheepfarms.Include(f => f.Products).FirstOrDefaultAsync(m => m.Id == id);

            if (farm == null) return NotFound();

            if (farm.OwnerId != currentUserId && !isModerator)
            {
                return Forbid();
            }

            if (farm.Products != null)
            {
                foreach (var product in farm.Products)
                {
                    if (!string.IsNullOrEmpty(product.ImagePath))
                    {
                        string filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", "products", product.ImagePath);
                        if (System.IO.File.Exists(filePath))
                        {
                            try { System.IO.File.Delete(filePath); } catch { }
                        }
                    }
                }
            }

            _context.Sheepfarms.Remove(farm);
            await _context.SaveChangesAsync();

            if (isModerator)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(nameof(YourFarms)); 
        }





        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3; 
            var phi1 = lat1 * Math.PI / 180;
            var phi2 = lat2 * Math.PI / 180;
            var deltaPhi = (lat2 - lat1) * Math.PI / 180;
            var deltaLambda = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                    Math.Cos(phi1) * Math.Cos(phi2) *
                    Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; 
        }

        [Authorize(Roles = "Shepherd")]
        [HttpGet]
        public async Task<IActionResult> Threats()
        {
            var currentUserId = _userManager.GetUserId(User);

            var myFarms = await _context.Sheepfarms.Where(f => f.OwnerId == currentUserId).ToListAsync();

            ViewBag.MyFarms = myFarms;

            var allThreats = await _context.Threats.Include(t => t.Reporter).Where(t => t.Status == ThreatStatus.Active).ToListAsync();

            var nearbyThreats = new List<Threat>();

            if (!myFarms.Any())
            {
                nearbyThreats = allThreats.Where(t => t.ReporterId == currentUserId).ToList();
            }
            else
            {
                foreach (var threat in allThreats)
                {
                    if (threat.ReporterId == currentUserId)
                    {
                        nearbyThreats.Add(threat);
                        continue;
                    }

                    foreach (var farm in myFarms)
                    {
                        if (CalculateDistance(farm.Latitude, farm.Longitude, threat.Latitude, threat.Longitude) <= 20000)
                        {
                            if (!nearbyThreats.Contains(threat)) nearbyThreats.Add(threat);
                            break;
                        }
                    }
                }
            }

            return View(nearbyThreats.OrderByDescending(t => t.TimeReported).ToList());
        }



        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> Threat(int id)
        {
            var threat = await _context.Threats.Include(t => t.Reporter).FirstOrDefaultAsync(t => t.Id == id);

            if (threat == null) return NotFound();

            return View(threat);
        }


        [Authorize(Roles = "Shepherd")]
        [HttpGet]
        public IActionResult AddThreat()
        {
            return View(new ThreatViewModel());
        }

        [Authorize(Roles = "Shepherd")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddThreat(ThreatViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var currentUserId = _userManager.GetUserId(User);
            string? uniqueFileName = null;

            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "threats");
                Directory.CreateDirectory(uploadsFolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }
            }

            var threat = new Threat
            {
                ReporterId = currentUserId,
                Type = model.Type,
                Description = model.Description,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Status = ThreatStatus.Active,
                MapIcon = (model.Type == ThreatType.Wolf) ? "icons/wolfIcon.png" : "icons/bearIcon.png",
                TimeReported = DateTimeOffset.UtcNow,
                ImagePath = uniqueFileName
            };

            _context.Threats.Add(threat);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Threats));
        }

        [Authorize(Roles = "Shepherd")]
        [HttpGet]
        public async Task<IActionResult> EditThreat(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var threat = await _context.Threats.FirstOrDefaultAsync(t => t.Id == id);

            if (threat == null) return NotFound();
            if (threat.ReporterId != currentUserId) return Forbid();

            var model = new ThreatViewModel
            {
                Id = threat.Id,
                Type = threat.Type,
                Description = threat.Description,
                Latitude = threat.Latitude,
                Longitude = threat.Longitude,
                ExistingImagePath = threat.ImagePath 
            };

            return View(model);
        }

        [Authorize(Roles = "Shepherd")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditThreat(ThreatViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var currentUserId = _userManager.GetUserId(User);
            var threat = await _context.Threats.FirstOrDefaultAsync(t => t.Id == model.Id);

            if (threat == null) return NotFound();
            if (threat.ReporterId != currentUserId) return Forbid();

            threat.Type = model.Type;
            threat.Description = model.Description;
            threat.Latitude = model.Latitude;
            threat.Longitude = model.Longitude;

            if (model.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(threat.ImagePath))
                {
                    string oldPath = Path.Combine(_hostEnvironment.WebRootPath, "images", "threats", threat.ImagePath);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "threats");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                threat.ImagePath = uniqueFileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Threats));
        }


        [Authorize(Roles = "Shepherd")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteThreat(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var threat = await _context.Threats.FirstOrDefaultAsync(t => t.Id == id);

            if (threat == null) return NotFound();

            var isModerator = User.IsInRole("Moderator");
            if (threat.ReporterId != currentUserId && !isModerator) return Forbid();

            if (!string.IsNullOrEmpty(threat.ImagePath))
            {
                string filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", "threats", threat.ImagePath);
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }

            _context.Threats.Remove(threat);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Threats));
        }

    }
}