using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StanaGO.Data; 
using StanaGO.Models;
using StanaGO.ViewModels;

namespace StanaGO.Controllers
{
    public class AccountController : Controller           // logica din spatele inregistrarii, autentificarii si deconectarii utilizatorilor
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly StanaGOContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, StanaGOContext context, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Register()       // actiunea pentru afișarea paginii de înregistrare 
        {
            if (User.Identity.IsAuthenticated)     // daca utilizatorul este deja autentificat e trimis direct la pagina principala
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)    // actiunea pentru procesarea datelor de înregistrare
        {
            if (ModelState.IsValid)
            {
                var userByName = await _userManager.FindByNameAsync(model.Username);
                if (userByName != null)
                {
                    ModelState.AddModelError(nameof(model.Username), "Acest nume de utilizator este deja folosit.");
                    return View(model);
                }
                var userByEmail = await _userManager.FindByEmailAsync(model.Email);
                if (userByEmail != null)
                {
                    ModelState.AddModelError(nameof(model.Email), "Acest email este deja înregistrat.");
                    return View(model);
                }

                var user = new Casual
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Username,
                    Email = model.Email,
                    RegistrationTime = DateTimeOffset.UtcNow,
                    Status = StanaGO.Enums.UserStatus.Online,
                    EmailConfirmed = true
                };

                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
                user.NormalizedUserName = _userManager.KeyNormalizer.NormalizeName(user.UserName);
                user.NormalizedEmail = _userManager.KeyNormalizer.NormalizeEmail(user.Email);
                user.SecurityStamp = Guid.NewGuid().ToString("D");
                user.ConcurrencyStamp = Guid.NewGuid().ToString("D");

                _context.Casuals.Add(user);


                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _userManager.AddToRoleAsync(user, "Casual");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Eroare la salvarea în baza de date.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()   // actiunea pentru afișarea paginii de autentificare
        {
            if (User.Identity.IsAuthenticated)    // daca utilizatorul este deja autentificat e trimis direct la pagina principala
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)  // actiunea pentru procesarea datelor de autentificare
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        user = await _userManager.FindByNameAsync(model.Email);
                    }

                    if (user != null)
                    {
                        var resultCheck = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

                        if (resultCheck.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Autentificare eșuată. Email/Username sau parolă invalidă.");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout() // actiunea pentru deconectarea utilizatorului
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [Authorize]
        [HttpGet]
        public IActionResult BecomeShepherd()   // actiunea pentru afișarea paginii de convertire a unui utilizator normal in utilizator tip cioban
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Shepherd"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new BecomeShepherdViewModel());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BecomeShepherd(BecomeShepherdViewModel model)   // actiunea pentru convertire 
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var casualUser = await _context.Users.OfType<Casual>().FirstOrDefaultAsync(u => u.Id == userId);

            if (casualUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var shepherdUser = new Shepherd
            {
                Id = casualUser.Id,
                UserName = casualUser.UserName,
                NormalizedUserName = casualUser.NormalizedUserName,
                Email = casualUser.Email,
                NormalizedEmail = casualUser.NormalizedEmail,
                PasswordHash = casualUser.PasswordHash,
                ConcurrencyStamp = casualUser.ConcurrencyStamp,
                SecurityStamp = Guid.NewGuid().ToString("D"),

                FirstName = casualUser.FirstName,
                LastName = casualUser.LastName,
                RegistrationTime = casualUser.RegistrationTime,
                Status = casualUser.Status,
                EmailConfirmed = casualUser.EmailConfirmed,

                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = true
            };

            _context.Users.Remove(casualUser);
            _context.Shepherds.Add(shepherdUser);

            var saveResult = await _context.SaveChangesAsync();

            if (saveResult > 0)
            {
                await _signInManager.SignOutAsync();
                await _userManager.RemoveFromRoleAsync(shepherdUser, "Casual");
                await _userManager.AddToRoleAsync(shepherdUser, "Shepherd");
                await _signInManager.SignInAsync(shepherdUser, isPersistent: false);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Eroare la conversia contului. Încearcă din nou.");
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile(string? id)
        {
            var currentLoggedUserId = _userManager.GetUserId(User);

            var targetUserId = id ?? currentLoggedUserId;

            var user = await _context.Users.Include(u => u.UserProfile).FirstOrDefaultAsync(u => u.Id == targetUserId);

            if (user == null) return NotFound();

            if (user.UserProfile == null)
            {
                if (targetUserId == currentLoggedUserId)
                {
                    return RedirectToAction(nameof(EditProfile));
                }

                return RedirectToAction("Login");
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewData["UserRole"] = roles.FirstOrDefault() ?? "Casual";

            ViewData["UserPhone"] = user.PhoneNumber;

            var userProducts = await _context.Products.Include(p => p.Farm).Where(p => p.Farm.OwnerId == targetUserId).OrderByDescending(p => p.Id).ToListAsync();

            ViewData["UserProducts"] = userProducts;

            return View(user.UserProfile);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Include(u => u.UserProfile).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return RedirectToAction("Login");

            var model = new ProfileViewModel();

            model.PhoneNumber = user.PhoneNumber;

            if (user.UserProfile != null)
            {
                model.FirstName = user.UserProfile.FirstName; 
                model.LastName = user.UserProfile.LastName;   
                model.Bio = user.UserProfile.Bio;
                model.Address = user.UserProfile.Address;
                model.CurrentProfilePicture = user.UserProfile.ImagePath;
            }
            else
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Include(u => u.UserProfile).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber; 

                if (user.UserProfile == null)
                {
                    user.UserProfile = new Profile { Id = userId, User = user };
                    _context.Profiles.Add(user.UserProfile);
                }

                user.UserProfile.FirstName = model.FirstName; 
                user.UserProfile.LastName = model.LastName;  
                user.UserProfile.Bio = model.Bio;
                user.UserProfile.Address = model.Address;

                if (model.ProfileImage != null)
                {
                    string folderPath = Path.Combine(_hostEnvironment.WebRootPath, "images", "profiles");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                    string filePath = Path.Combine(folderPath, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfileImage.CopyToAsync(fileStream);
                    }
                    user.UserProfile.ImagePath = uniqueFileName;
                }

                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync(); 

                return RedirectToAction(nameof(Profile));
            }

            return View(model);
        }
    }
}