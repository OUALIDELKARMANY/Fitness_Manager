using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Fitness_Manager.Models;

namespace Fitness_Manager.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard principal du client
        public IActionResult Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var user = _context.Utilisateurs.FirstOrDefault(u => u.Id == int.Parse(userId));
                ViewBag.UserName = user?.Prenom ?? "Client";
            }
            else
            {
                ViewBag.UserName = "Client";
            }
            return View();
        }

        // Gestion du profil
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == userId);
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string prenom, string nom, string email, 
            decimal? poids, decimal? taille, int? age, string? sexe, string? objectif)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == userId);

            if (client == null)
            {
                return NotFound();
            }

            // Mettre à jour les propriétés
            client.Prenom = prenom;
            client.Nom = nom;
            client.Email = email;
            client.Poids = poids;
            client.Taille = taille;
            client.Age = age;
            client.Sexe = sexe;
            client.Objectif = objectif;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Profil mis à jour avec succès!";
            return RedirectToAction(nameof(Profile));
        }

        // Plan sportif
        public async Task<IActionResult> WorkoutPlan()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var client = await _context.Clients
                .Include(c => c.PlanSportif)
                    .ThenInclude(p => p!.Seances)
                        .ThenInclude(s => s.Exercices)
                .FirstOrDefaultAsync(c => c.Id == userId);
            
            return View(client);
        }

        // Plan nutritionnel
        public async Task<IActionResult> NutritionPlan()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var client = await _context.Clients
                .Include(c => c.PlanNutritionnel)
                    .ThenInclude(p => p!.Aliments)
                .FirstOrDefaultAsync(c => c.Id == userId);
            
            return View(client);
        }

        // Progression
        public async Task<IActionResult> Progress()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var client = await _context.Clients
                .Include(c => c.SuiviPoids.OrderBy(sp => sp.DateMesure))
                .Include(c => c.PlanSportif)
                    .ThenInclude(p => p!.Seances)
                .FirstOrDefaultAsync(c => c.Id == userId);
            
            if (client == null)
            {
                return NotFound();
            }

            // Préparer les données pour la vue
            ViewBag.Client = client;
            ViewBag.PoidsActuel = client.SuiviPoids.LastOrDefault()?.Poids ?? client.Poids;
            ViewBag.PoidsInitial = client.SuiviPoids.FirstOrDefault()?.Poids ?? client.Poids;
            ViewBag.NombreMesures = client.SuiviPoids.Count;
            ViewBag.NombreSeances = client.PlanSportif?.Seances.Count ?? 0;
            
            return View(client);
        }
    }
}
