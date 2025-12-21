using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Fitness_Manager.Models;

namespace Fitness_Manager.Controllers
{
    [Authorize(Roles = "Coach")]
    public class CoachController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoachController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================
        // DASHBOARD & OVERVIEW
        // ============================================

        public async Task<IActionResult> Dashboard()
        {
            var coachId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var coach = await _context.Coachs
                .FirstOrDefaultAsync(c => c.Id == coachId);

            if (coach == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Statistics - TOUS les clients (pas seulement ceux du coach)
            ViewBag.CoachName = $"{coach.Prenom} {coach.Nom}";
            ViewBag.TotalClients = await _context.Clients.CountAsync();
            ViewBag.ActiveWorkoutPlans = await _context.PlansSportifs.CountAsync();
            ViewBag.ActiveNutritionPlans = await _context.PlansNutritionnels.CountAsync();
            ViewBag.TotalReports = await _context.Rapports.CountAsync();

            // Recent clients - TOUS les clients récents
            ViewBag.RecentClients = await _context.Clients
                .OrderByDescending(c => c.DateInscription)
                .Take(5)
                .ToListAsync();

            return View();
        }

        // ============================================
        // CLIENT MANAGEMENT
        // ============================================

        public async Task<IActionResult> Clients(string? search)
        {
            // TOUS les clients (pas de filtre par CoachId)
            var clients = _context.Clients
                .Include(c => c.PlanSportif)
                .Include(c => c.PlanNutritionnel)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                clients = clients.Where(c => 
                    c.Nom.Contains(search) || 
                    c.Prenom.Contains(search) || 
                    c.Email.Contains(search));
            }

            var clientList = await clients.OrderBy(c => c.Nom).ToListAsync();
            ViewBag.SearchQuery = search;
            
            return View(clientList);
        }

        public async Task<IActionResult> ClientDetails(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Coach)
                .Include(c => c.PlanSportif)
                    .ThenInclude(p => p!.Seances)
                .Include(c => c.PlanNutritionnel)
                    .ThenInclude(p => p!.Aliments)
                .Include(c => c.SuiviPoids)
                .Include(c => c.Rapports)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            // Get available plans for assignment
            ViewBag.AvailableWorkoutPlans = await _context.PlansSportifs.ToListAsync();
            ViewBag.AvailableNutritionPlans = await _context.PlansNutritionnels.ToListAsync();

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPlans(int clientId, int? workoutPlanId, int? nutritionPlanId)
        {
            var client = await _context.Clients.FindAsync(clientId);
            
            if (client == null)
            {
                return NotFound();
            }

            client.PlanSportifId = workoutPlanId;
            client.PlanNutritionnelId = nutritionPlanId;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Plans assignés avec succès!";
            return RedirectToAction(nameof(ClientDetails), new { id = clientId });
        }

        public async Task<IActionResult> EditClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            
            if (client == null)
            {
                return NotFound();
            }
            
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClient(Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Update(client);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Client mis à jour avec succès!";
                return RedirectToAction(nameof(ClientDetails), new { id = client.Id });
            }
            
            return View(client);
        }

        public IActionResult AddClient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClient(Client client)
        {
            if (ModelState.IsValid)
            {
                var coachId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                
                // Vérifier si l'email existe déjà
                var existingUser = await _context.Utilisateurs
                    .FirstOrDefaultAsync(u => u.Email == client.Email);
                
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Cet email est déjà utilisé.");
                    return View(client);
                }
                
                // Créer le client et l'assigner au coach
                client.CoachId = coachId;
                client.DateInscription = DateTime.Now;
                client.DateCreation = DateTime.Now;
                
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Client ajouté avec succès!";
                return RedirectToAction(nameof(Clients));
            }
            
            return View(client);
        }

        // ============================================
        // WORKOUT PLAN MANAGEMENT
        // ============================================

        public async Task<IActionResult> WorkoutPlans()
        {
            var plans = await _context.PlansSportifs
                .Include(p => p.Seances)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            
            return View(plans);
        }

        public async Task<IActionResult> CreateWorkoutPlan()
        {
            var coachId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Récupérer tous les clients pour le dropdown
            ViewBag.Clients = await _context.Clients
                .OrderBy(c => c.Nom)
                .ThenBy(c => c.Prenom)
                .ToListAsync();
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorkoutPlan(PlanSportif plan, int? clientId)
        {
            if (ModelState.IsValid)
            {
                _context.PlansSportifs.Add(plan);
                await _context.SaveChangesAsync();
                
                // Si un client a été sélectionné, assigner automatiquement le plan
                if (clientId.HasValue)
                {
                    var client = await _context.Clients.FindAsync(clientId.Value);
                    if (client != null)
                    {
                        client.PlanSportifId = plan.Id;
                        await _context.SaveChangesAsync();
                        TempData["Success"] = $"Plan d'entraînement créé et assigné à {client.Prenom} {client.Nom}!";
                    }
                    else
                    {
                        TempData["Success"] = "Plan d'entraînement créé avec succès!";
                    }
                }
                else
                {
                    TempData["Success"] = "Plan d'entraînement créé avec succès!";
                }
                
                return RedirectToAction(nameof(ManageSessions), new { planId = plan.Id });
            }
            
            // Recharger les clients en cas d'erreur
            ViewBag.Clients = await _context.Clients
                .OrderBy(c => c.Nom)
                .ThenBy(c => c.Prenom)
                .ToListAsync();
            
            return View(plan);
        }

        public async Task<IActionResult> EditWorkoutPlan(int id)
        {
            var plan = await _context.PlansSportifs.FindAsync(id);
            
            if (plan == null)
            {
                return NotFound();
            }
            
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorkoutPlan(PlanSportif plan)
        {
            if (ModelState.IsValid)
            {
                _context.Update(plan);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Plan d'entraînement mis à jour!";
                return RedirectToAction(nameof(WorkoutPlans));
            }
            
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorkoutPlan(int id)
        {
            var plan = await _context.PlansSportifs.FindAsync(id);
            
            if (plan != null)
            {
                _context.PlansSportifs.Remove(plan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Plan supprimé avec succès!";
            }
            
            return RedirectToAction(nameof(WorkoutPlans));
        }

        // ============================================
        // SESSION MANAGEMENT
        // ============================================

        public async Task<IActionResult> ManageSessions(int planId)
        {
            var plan = await _context.PlansSportifs
                .Include(p => p.Seances)
                    .ThenInclude(s => s.Exercices)
                .FirstOrDefaultAsync(p => p.Id == planId);

            if (plan == null)
            {
                return NotFound();
            }

            ViewBag.PlanId = planId;
            ViewBag.PlanName = plan.Nom;
            
            return View(plan.Seances.OrderBy(s => s.Ordre).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSession(int planId, Seance seance)
        {
            seance.PlanSportifId = planId;
            
            _context.Seances.Add(seance);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Séance ajoutée!";
            return RedirectToAction(nameof(ManageSessions), new { planId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSession(int id, int planId)
        {
            var seance = await _context.Seances.FindAsync(id);
            
            if (seance != null)
            {
                _context.Seances.Remove(seance);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Séance supprimée!";
            }
            
            return RedirectToAction(nameof(ManageSessions), new { planId });
        }

        public async Task<IActionResult> EditSession(int id)
        {
            var seance = await _context.Seances
                .Include(s => s.PlanSportif)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if (seance == null)
            {
                return NotFound();
            }

            ViewBag.PlanId = seance.PlanSportifId;
            return View(seance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSession(Seance seance)
        {
            if (ModelState.IsValid)
            {
                _context.Update(seance);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Séance mise à jour!";
                return RedirectToAction(nameof(ManageSessions), new { planId = seance.PlanSportifId });
            }
            
            return View(seance);
        }

        // ============================================
        // EXERCISE MANAGEMENT
        // ============================================

        public async Task<IActionResult> ManageExercises(int sessionId)
        {
            var seance = await _context.Seances
                .Include(s => s.Exercices)
                .Include(s => s.PlanSportif)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (seance == null)
            {
                return NotFound();
            }

            ViewBag.SessionId = sessionId;
            ViewBag.SessionName = seance.Nom;
            ViewBag.PlanId = seance.PlanSportifId;
            
            return View(seance.Exercices.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExercise(int sessionId, Exercice exercice)
        {
            exercice.SeanceId = sessionId;
            
            _context.Exercices.Add(exercice);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Exercice ajouté!";
            return RedirectToAction(nameof(ManageExercises), new { sessionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExercise(int id, int sessionId)
        {
            var exercice = await _context.Exercices.FindAsync(id);
            
            if (exercice != null)
            {
                _context.Exercices.Remove(exercice);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Exercice supprimé!";
            }
            
            return RedirectToAction(nameof(ManageExercises), new { sessionId });
        }

        public async Task<IActionResult> EditExercise(int id)
        {
            var exercice = await _context.Exercices
                .Include(e => e.Seance)
                .FirstOrDefaultAsync(e => e.Id == id);
            
            if (exercice == null)
            {
                return NotFound();
            }

            ViewBag.SessionId = exercice.SeanceId;
            return View(exercice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExercise(Exercice exercice)
        {
            if (ModelState.IsValid)
            {
                _context.Update(exercice);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Exercice mis à jour!";
                return RedirectToAction(nameof(ManageExercises), new { sessionId = exercice.SeanceId });
            }
            
            return View(exercice);
        }

        // ============================================
        // NUTRITION PLAN MANAGEMENT
        // ============================================

        public async Task<IActionResult> NutritionPlans()
        {
            var plans = await _context.PlansNutritionnels
                .Include(p => p.Aliments)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            
            return View(plans);
        }

        public async Task<IActionResult> CreateNutritionPlan()
        {
            var coachId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Récupérer tous les clients pour le dropdown
            ViewBag.Clients = await _context.Clients
                .OrderBy(c => c.Nom)
                .ThenBy(c => c.Prenom)
                .ToListAsync();
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNutritionPlan(PlanNutritionnel plan, int? clientId)
        {
            if (ModelState.IsValid)
            {
                _context.PlansNutritionnels.Add(plan);
                await _context.SaveChangesAsync();
                
                // Si un client a été sélectionné, assigner automatiquement le plan
                if (clientId.HasValue)
                {
                    var client = await _context.Clients.FindAsync(clientId.Value);
                    if (client != null)
                    {
                        client.PlanNutritionnelId = plan.Id;
                        await _context.SaveChangesAsync();
                        TempData["Success"] = $"Plan nutritionnel créé et assigné à {client.Prenom} {client.Nom}!";
                    }
                    else
                    {
                        TempData["Success"] = "Plan nutritionnel créé avec succès!";
                    }
                }
                else
                {
                    TempData["Success"] = "Plan nutritionnel créé avec succès!";
                }
                
                return RedirectToAction(nameof(ManageFoods), new { planId = plan.Id });
            }
            
            // Recharger les clients en cas d'erreur
            ViewBag.Clients = await _context.Clients
                .OrderBy(c => c.Nom)
                .ThenBy(c => c.Prenom)
                .ToListAsync();
            
            return View(plan);
        }

        public async Task<IActionResult> EditNutritionPlan(int id)
        {
            var plan = await _context.PlansNutritionnels.FindAsync(id);
            
            if (plan == null)
            {
                return NotFound();
            }
            
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNutritionPlan(PlanNutritionnel plan)
        {
            if (ModelState.IsValid)
            {
                _context.Update(plan);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Plan nutritionnel mis à jour!";
                return RedirectToAction(nameof(NutritionPlans));
            }
            
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNutritionPlan(int id)
        {
            var plan = await _context.PlansNutritionnels.FindAsync(id);
            
            if (plan != null)
            {
                _context.PlansNutritionnels.Remove(plan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Plan supprimé avec succès!";
            }
            
            return RedirectToAction(nameof(NutritionPlans));
        }

        // ============================================
        // FOOD MANAGEMENT
        // ============================================

        public async Task<IActionResult> ManageFoods(int planId)
        {
            var plan = await _context.PlansNutritionnels
                .Include(p => p.Aliments)
                .FirstOrDefaultAsync(p => p.Id == planId);

            if (plan == null)
            {
                return NotFound();
            }

            ViewBag.PlanId = planId;
            ViewBag.PlanName = plan.Nom;
            
            return View(plan.Aliments.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFood(int planId, Aliment aliment)
        {
            aliment.PlanNutritionnelId = planId;
            
            _context.Aliments.Add(aliment);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Aliment ajouté!";
            return RedirectToAction(nameof(ManageFoods), new { planId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFood(int id, int planId)
        {
            var aliment = await _context.Aliments.FindAsync(id);
            
            if (aliment != null)
            {
                _context.Aliments.Remove(aliment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Aliment supprimé!";
            }
            
            return RedirectToAction(nameof(ManageFoods), new { planId });
        }

        public async Task<IActionResult> EditFood(int id)
        {
            var aliment = await _context.Aliments
                .Include(a => a.PlanNutritionnel)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (aliment == null)
            {
                return NotFound();
            }

            ViewBag.PlanId = aliment.PlanNutritionnelId;
            return View(aliment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFood(Aliment aliment)
        {
            if (ModelState.IsValid)
            {
                _context.Update(aliment);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Aliment mis à jour!";
                return RedirectToAction(nameof(ManageFoods), new { planId = aliment.PlanNutritionnelId });
            }
            
            return View(aliment);
        }

        // ============================================
        // PROGRESS & REPORTS
        // ============================================

        public async Task<IActionResult> ClientProgress(int clientId)
        {
            var client = await _context.Clients
                .Include(c => c.SuiviPoids)
                .Include(c => c.PlanSportif)
                .Include(c => c.PlanNutritionnel)
                .FirstOrDefaultAsync(c => c.Id == clientId);

            if (client == null)
            {
                return NotFound();
            }

            ViewBag.ClientName = $"{client.Prenom} {client.Nom}";
            
            return View(client);
        }

        public async Task<IActionResult> Reports()
        {
            var coachId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var reports = await _context.Rapports
                .Include(r => r.Client)
                .Where(r => r.CoachId == coachId)
                .OrderByDescending(r => r.DateCreation)
                .ToListAsync();
            
            return View(reports);
        }

        public async Task<IActionResult> CreateReport(int clientId)
        {
            var client = await _context.Clients.FindAsync(clientId);
            
            if (client == null)
            {
                return NotFound();
            }

            ViewBag.ClientId = clientId;
            ViewBag.ClientName = $"{client.Prenom} {client.Nom}";
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReport(Rapport rapport)
        {
            if (ModelState.IsValid)
            {
                var coachId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                rapport.CoachId = coachId;
                rapport.DateCreation = DateTime.Now;
                
                _context.Rapports.Add(rapport);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Rapport créé avec succès!";
                return RedirectToAction(nameof(Reports));
            }
            
            return View(rapport);
        }

        public async Task<IActionResult> ViewReport(int id)
        {
            var rapport = await _context.Rapports
                .Include(r => r.Client)
                .Include(r => r.Coach)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rapport == null)
            {
                return NotFound();
            }
            
            return View(rapport);
        }
    }
}
