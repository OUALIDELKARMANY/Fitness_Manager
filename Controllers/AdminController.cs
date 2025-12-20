using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness_Manager.Models;

namespace Fitness_Manager.Controllers
{
    // CONTROLLER TEMPORAIRE POUR CRÉER DES COMPTES COACH
    // À SUPPRIMER EN PRODUCTION !
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/CreateCoach
        public IActionResult CreateCoach()
        {
            return View();
        }

        // POST: /Admin/CreateCoach
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCoach(string email, string password, string nom, string prenom, string? specialite)
        {
            try
            {
                // Vérifier si l'email existe déjà
                var existingUser = await _context.Utilisateurs
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (existingUser != null)
                {
                    ViewBag.Error = "Cet email est déjà utilisé.";
                    return View();
                }

                // Créer le coach
                var coach = new Coach
                {
                    Email = email,
                    Password = password, // NOTE: En production, utilisez un hash!
                    Nom = nom,
                    Prenom = prenom,
                    Specialite = specialite,
                    DateCreation = DateTime.Now
                };

                _context.Coachs.Add(coach);
                await _context.SaveChangesAsync();

                ViewBag.Success = $"Coach créé avec succès! Email: {email}, Password: {password}";
                ViewBag.LoginUrl = Url.Action("Login", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Erreur: {ex.Message}";
            }

            return View();
        }
    }
}
