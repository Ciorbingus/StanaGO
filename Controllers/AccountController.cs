using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StanaGO.Data; 
using StanaGO.Models;
using StanaGO.ViewModels;

namespace StanaGO.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly StanaGOContext _context; 

        public AccountController (
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            StanaGOContext context )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context; 
        }

        [HttpGet]
        public IActionResult Register ( )
        {
            if ( User.Identity.IsAuthenticated )
            {
                return RedirectToAction ("Index", "Home");
            }
            return View ();
        }

        
        [HttpPost]
        public async Task<IActionResult> Register ( RegisterViewModel model )
        {
            if ( ModelState.IsValid )
            {
                var userByName = await _userManager.FindByNameAsync (model.Username);
                if ( userByName != null )
                {
                    ModelState.AddModelError (nameof (model.Username), "Acest nume de utilizator este deja folosit.");
                    return View (model);
                }
                var userByEmail = await _userManager.FindByEmailAsync (model.Email);
                if ( userByEmail != null )
                {
                    ModelState.AddModelError (nameof (model.Email), "Acest email este deja înregistrat.");
                    return View (model);
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

                var passwordHasher = new PasswordHasher<User> ();
                user.PasswordHash = passwordHasher.HashPassword (user, model.Password);
                user.NormalizedUserName = _userManager.KeyNormalizer.NormalizeName (user.UserName);
                user.NormalizedEmail = _userManager.KeyNormalizer.NormalizeEmail (user.Email);
                user.SecurityStamp = Guid.NewGuid ().ToString ("D");
                user.ConcurrencyStamp = Guid.NewGuid ().ToString ("D");


                _context.Casuals.Add (user);
              

                var result = await _context.SaveChangesAsync ();

                if ( result > 0 )
                {
                    await _signInManager.SignInAsync (user, isPersistent: false);
                    return RedirectToAction ("Index", "Home");
                }

                ModelState.AddModelError (string.Empty, "Eroare la salvarea în baza de date.");
            }
            return View (model);
        }

        [HttpGet]
        public IActionResult Login ( )
        {
            if ( User.Identity.IsAuthenticated )
            {
                return RedirectToAction ("Index", "Home");
            }
            return View ();
        }

        [HttpPost]
        public async Task<IActionResult> Login ( LoginViewModel model )
        {
            if ( ModelState.IsValid )
            {
                var result = await _signInManager.PasswordSignInAsync (
                    model.Email,
                    model.Password,
                    model.RememberMe, 
                    lockoutOnFailure: false);

                if ( result.Succeeded )
                {
                    return RedirectToAction ("Index", "Home");
                }
                else
                {
                    var user = await _userManager.FindByEmailAsync (model.Email);
                    if ( user == null )
                    {
                        user = await _userManager.FindByNameAsync (model.Email);
                    }

                    if ( user != null )
                    {
                        var resultCheck = await _signInManager.CheckPasswordSignInAsync (user, model.Password, lockoutOnFailure: false);

                        if ( resultCheck.Succeeded )
                        {
                            await _signInManager.SignInAsync (user, isPersistent: model.RememberMe);
                            return RedirectToAction ("Index", "Home");
                        }
                    }

                    ModelState.AddModelError (string.Empty, "Autentificare eșuată. Email/Username sau parolă invalidă.");
                    return View (model);
                }
            }
            return View (model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout ( )
        {
            await _signInManager.SignOutAsync ();
            return RedirectToAction ("Index", "Home");
        }
    }
}