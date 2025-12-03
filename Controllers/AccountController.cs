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

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, StanaGOContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
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
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 1️⃣ Căutăm profilul în tabela Profiles
            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == user.Id);

            // 2️⃣ Dacă nu există — îl creăm!
            if (profile == null)
            {
                profile = new Profile
                {
                    Id = user.Id,
                    AvatarUrl = "/images/default-avatar.png",
                    LocationText = "",
                    DateOfBirth = new DateTime(2000, 1, 1)
                };

                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            // 3️⃣ Mapăm datele în ViewModel
            var model = new ProfileViewModel
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Latitude = user.Latitude,
                Longitude = user.Longitude,

                AvatarUrl = profile.AvatarUrl,
                LocationText = profile.LocationText,
                Bio = profile.Bio,
                DateOfBirth = profile.DateOfBirth
            };

            return View(model);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == user.Id);

            var model = new EditProfileViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = profile?.AvatarUrl,
                LocationText = profile?.LocationText,
                Bio = profile?.Bio
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]

        public async Task <IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.Username;
            user.NormalizedUserName = _userManager.NormalizeName(model.Username);
            user.Email = model.Email;
            user.NormalizedEmail = _userManager.NormalizeEmail(model.Email);
            user.PhoneNumber = model.PhoneNumber;


            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == user.Id);

            if (profile != null)
            {
                profile.AvatarUrl = model.AvatarUrl;
                profile.LocationText = model.LocationText;
                profile.Bio = model.Bio;

            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Profile");
        }
    }
}