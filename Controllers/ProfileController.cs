using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness_Manager.Models;

namespace Fitness_Manager.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Récupérer l'utilisateur
            var utilisateur = await _context.Utilisateurs.FindAsync(userId);
            if (utilisateur == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Auth");
            }

            // Déterminer si c'est un client ou un coach
            var client = await _context.Clients.FindAsync(userId);
            var coach = await _context.Coachs.FindAsync(userId);

            if (client != null)
            {
                ViewBag.UserType = "Client";
                ViewBag.Client = client;

                // Récupérer les données du client
                var suiviPoids = await _context.SuiviPoids
                    .Where(s => s.ClientId == userId)
                    .OrderByDescending(s => s.Date)
                    .Take(10)
                    .ToListAsync();

                ViewBag.SuiviPoids = suiviPoids;
            }
            else if (coach != null)
            {
                ViewBag.UserType = "Coach";
                ViewBag.Coach = coach;

                // Récupérer les clients du coach
                var clients = await _context.Clients
                    .Where(c => c.CoachId == userId)
                    .ToListAsync();

                ViewBag.Clients = clients;
            }

            return View(utilisateur);
        }

        // GET: /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var utilisateur = await _context.Utilisateurs.FindAsync(userId);
            if (utilisateur == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(utilisateur);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Utilisateur model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var utilisateur = await _context.Utilisateurs.FindAsync(userId);
                if (utilisateur == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Mettre à jour les propriétés
                utilisateur.Nom = model.Nom;
                utilisateur.Prenom = model.Prenom;
                utilisateur.Email = model.Email;
                utilisateur.Sexe = model.Sexe;
                utilisateur.Photo = model.Photo;

                // Si c'est un client, mettre à jour les infos supplémentaires
                var client = await _context.Clients.FindAsync(userId);
                if (client != null)
                {
                    // Créer un objet dynamique pour éviter les erreurs de cast
                    var clientProperties = model.GetType().GetProperties();

                    // Vérifier et mettre à jour Age si présent
                    var ageProperty = clientProperties.FirstOrDefault(p => p.Name == "Age");
                    if (ageProperty != null)
                    {
                        var ageValue = ageProperty.GetValue(model);
                        if (ageValue != null)
                        {
                            client.Age = Convert.ToInt32(ageValue);
                        }
                    }

                    // Vérifier et mettre à jour Taille si présent - CORRIGÉ
                    var tailleProperty = clientProperties.FirstOrDefault(p => p.Name == "Taille");
                    if (tailleProperty != null)
                    {
                        var tailleValue = tailleProperty.GetValue(model);
                        if (tailleValue != null)
                        {
                            client.Taille = Convert.ToDecimal(tailleValue); // CORRECTION ICI
                        }
                    }

                    // Vérifier et mettre à jour PoidsActuel si présent - CORRIGÉ
                    var poidsProperty = clientProperties.FirstOrDefault(p => p.Name == "PoidsActuel");
                    if (poidsProperty != null)
                    {
                        var poidsValue = poidsProperty.GetValue(model);
                        if (poidsValue != null)
                        {
                            client.PoidsActuel = Convert.ToDecimal(poidsValue); // CORRECTION ICI
                        }
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Profil mis à jour avec succès!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // POST: /Profile/AddWeight
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWeight(double poids, DateTime date)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var suiviPoids = new SuiviPoids
            {
                ClientId = userId.Value,
                Poids = Convert.ToDecimal(poids), // CORRECTION ICI
                Date = date
            };

            _context.SuiviPoids.Add(suiviPoids);
            await _context.SaveChangesAsync();

            // Mettre à jour le poids actuel du client
            var client = await _context.Clients.FindAsync(userId);
            if (client != null)
            {
                client.PoidsActuel = Convert.ToDecimal(poids); // CORRECTION ICI
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Poids enregistré avec succès!";
            return RedirectToAction("Index");
        }
    }
}