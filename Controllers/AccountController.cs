using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness_Manager.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Fitness_Manager.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            // Si l'utilisateur est déjà connecté, rediriger vers le dashboard approprié
            if (User.Identity?.IsAuthenticated == true)
            {
                // Vérifier le rôle et rediriger vers le bon dashboard
                if (User.IsInRole("Client"))
                {
                    return RedirectToAction("Dashboard", "Client");
                }
                else if (User.IsInRole("Coach"))
                {
                    return RedirectToAction("Dashboard", "Coach");
                }
            }
            return View();
        }

        // POST: /Account/Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Veuillez remplir tous les champs.";
                    return View();
                }

                // Rechercher l'utilisateur par email
                var utilisateur = await _context.Utilisateurs
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (utilisateur == null)
                {
                    ViewBag.Error = "Email ou mot de passe incorrect.";
                    return View();
                }

                // Vérification du mot de passe
                if (string.IsNullOrEmpty(utilisateur.Password) || utilisateur.Password != password)
                {
                    ViewBag.Error = "Email ou mot de passe incorrect.";
                    return View();
                }

                // Créer les claims (informations de l'utilisateur)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, utilisateur.Id.ToString()),
                    new Claim(ClaimTypes.Email, utilisateur.Email),
                    new Claim(ClaimTypes.Name, $"{utilisateur.Nom} {utilisateur.Prenom}"),
                    new Claim("UserId", utilisateur.Id.ToString())
                };

                // Ajouter le rôle selon le type d'utilisateur
                if (utilisateur is Client)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Client"));
                }
                else if (utilisateur is Coach)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Coach"));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
                };

                // Connecter l'utilisateur
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Rediriger selon le rôle
                if (utilisateur is Client)
                {
                    return RedirectToAction("Dashboard", "Client");
                }
                else if (utilisateur is Coach)
                {
                    return RedirectToAction("Dashboard", "Coach");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Une erreur est survenue: {ex.Message}";
                return View();
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string nom, string prenom)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(prenom))
                {
                    ViewBag.Error = "Veuillez remplir tous les champs obligatoires.";
                    return View();
                }

                // Vérifier si l'email existe déjà
                var existingUser = await _context.Utilisateurs
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (existingUser != null)
                {
                    ViewBag.Error = "Cet email est déjà utilisé.";
                    return View();
                }

                // Créer toujours un Client (seuls les clients peuvent s'inscrire)
                var client = new Client
                {
                    Email = email,
                    Password = password ?? string.Empty,
                    Nom = nom,
                    Prenom = prenom,
                    DateInscription = DateTime.Now,
                    DateCreation = DateTime.Now
                };
                _context.Clients.Add(client);

                await _context.SaveChangesAsync();

                ViewBag.Success = "Inscription réussie! Vous pouvez maintenant vous connecter.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Erreur lors de l'inscription: {ex.Message}";
                return View();
            }
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}