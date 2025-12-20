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
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var user = _context.Utilisateurs.FirstOrDefault(u => u.Id == int.Parse(userId));
                return View(user);
            }
            return View();
        }

        // Plan sportif
        public IActionResult WorkoutPlan()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // TODO: Récupérer le plan sportif de l'utilisateur depuis la base de données
            
            return View();
        }

        // Plan nutritionnel
        public IActionResult NutritionPlan()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // TODO: Récupérer le plan nutritionnel de l'utilisateur depuis la base de données
            
            return View();
        }

        // Progression
        public IActionResult Progress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // TODO: Récupérer les données de progression depuis la base de données
            
            return View();
        }
    }
}
