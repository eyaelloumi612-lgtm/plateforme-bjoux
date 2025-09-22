using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using JewelryManagementPlatform.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JewelryManagementPlatform.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
          
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterViewModel());
            
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState est valide"); // Debug
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    Console.WriteLine($"Utilisateur créé avec succès: {user.Email}"); // Debug
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
                // Log des erreurs détaillées
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Erreur création utilisateur: {error.Code} - {error.Description}"); // Debug
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                // Log des erreurs de validation
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Validation Error: {state.Key} - {error.ErrorMessage}");
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Initialiser le ViewModel avec le returnUrl
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            Console.WriteLine($"Login GET - ReturnUrl: {returnUrl}");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // DEBUG
            Console.WriteLine($"Login POST - ReturnUrl: {model.ReturnUrl}");

            if (!ModelState.IsValid)
            {
                // Afficher les erreurs de validation
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Validation Error: {state.Key} - {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            try
            {
                // Trouver l'utilisateur par email
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect");
                    return View(model);
                }

                // Tenter la connexion
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Redirection
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("Index", "Clients");
                }

                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur connexion: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Erreur lors de la connexion");
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            Console.WriteLine("Tentative de déconnexion...");
            await _signInManager.SignOutAsync();
            Console.WriteLine("Déconnexion réussie");
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}