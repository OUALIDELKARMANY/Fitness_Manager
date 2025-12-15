using Microsoft.EntityFrameworkCore;

namespace Fitness_Manager.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Coach> Coachs { get; set; }
        public DbSet<PlanSportif> PlansSportifs { get; set; }
        public DbSet<Seance> Seances { get; set; }
        public DbSet<Exercice> Exercices { get; set; }
        public DbSet<CoachPlanSportif> CoachPlanSportifs { get; set; }
        public DbSet<PlanNutritionnel> PlansNutritionnels { get; set; }
        public DbSet<Aliment> Aliments { get; set; }
        public DbSet<SuiviPoids> SuiviPoids { get; set; }
        public DbSet<Rapport> Rapports { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Clé composite pour l’association CoachPlanSportif
            modelBuilder.Entity<CoachPlanSportif>()
                .HasKey(cps => new { cps.CoachId, cps.PlanSportifId });

            // Relations many-to-many Aliment ↔ PlanNutritionnel
            modelBuilder.Entity<PlanNutritionnel>()
                .HasMany(p => p.Aliments)
                .WithMany(a => a.PlansNutritionnels);
        }
    }
}
