using Microsoft.EntityFrameworkCore;
using Fitness_Manager.Models;

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
            base.OnModelCreating(modelBuilder);

            // ==================== CONFIGURATION DE L'HÉRITAGE ====================
            // Configuration de l'héritage TPH (Table Per Hierarchy) pour Utilisateur
            modelBuilder.Entity<Utilisateur>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Utilisateur>("Utilisateur")
                .HasValue<Client>("Client")
                .HasValue<Coach>("Coach");

            // ==================== CONFIGURATION UTILISATEUR ====================
            modelBuilder.Entity<Utilisateur>(entity =>
            {
                entity.HasKey(u => u.Id);

                // Configuration des propriétés
                entity.Property(u => u.Nom)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Prenom)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                // AJOUT DES NOUVELLES PROPRIÉTÉS
                entity.Property(u => u.Sexe)
                    .HasMaxLength(10);

                entity.Property(u => u.Photo)
                    .HasMaxLength(500);

                entity.Property(u => u.DateCreation)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Contrainte d'unicité sur l'email
                entity.HasIndex(u => u.Email)
                    .IsUnique();
            });

            // ==================== CONFIGURATION CLIENT ====================
            modelBuilder.Entity<Client>(entity =>
            {
                // Relation Many-to-One avec Coach
                entity.HasOne(c => c.Coach)
                      .WithMany(coach => coach.Clients)
                      .HasForeignKey(c => c.CoachId)
                      .OnDelete(DeleteBehavior.SetNull);

                // Relations avec PlanSportif et PlanNutritionnel
                entity.HasOne(c => c.PlanSportif)
                      .WithMany()
                      .HasForeignKey(c => c.PlanSportifId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(c => c.PlanNutritionnel)
                      .WithMany()
                      .HasForeignKey(c => c.PlanNutritionnelId)
                      .OnDelete(DeleteBehavior.SetNull);

                // Configuration des propriétés spécifiques au Client
                entity.Property(c => c.DateInscription)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // AJOUT DES NOUVELLES PROPRIÉTÉS
                entity.Property(c => c.Age)
                    .HasDefaultValue(0);

                entity.Property(c => c.Poids)
                    .HasColumnType("decimal(5,2)");

                entity.Property(c => c.PoidsActuel) // AJOUTÉ
                    .HasColumnType("decimal(5,2)");

                entity.Property(c => c.Taille)
                    .HasColumnType("decimal(4,1)");

                entity.Property(c => c.Objectif)
                    .HasMaxLength(500);

                entity.Property(c => c.FrequenceEntrainement)
                    .HasDefaultValue(3);
            });

            // ==================== CONFIGURATION COACH ====================
            modelBuilder.Entity<Coach>(entity =>
            {
                // Relation One-to-Many avec Clients (déjà configurée côté Client)

                // Relation Many-to-Many avec PlansSportifs via CoachPlanSportif
                entity.HasMany(c => c.CoachPlanSportifs)
                      .WithOne(cps => cps.Coach)
                      .HasForeignKey(cps => cps.CoachId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Configuration des propriétés spécifiques au Coach
                entity.Property(c => c.Specialite)
                    .HasMaxLength(200);

                entity.Property(c => c.Certifications)
                    .HasMaxLength(500);

                entity.Property(c => c.Disponibilites)
                    .HasMaxLength(500);

                entity.Property(c => c.TarifHoraire)
                    .HasColumnType("decimal(8,2)");

                entity.Property(c => c.AnneesExperience)
                    .HasDefaultValue(0);
            });

            // ==================== CONFIGURATION PLAN SPORTIF ====================
            modelBuilder.Entity<PlanSportif>(entity =>
            {
                entity.HasKey(p => p.Id);

                // Relation One-to-Many avec Seances
                entity.HasMany(p => p.Seances)
                      .WithOne(s => s.PlanSportif)
                      .HasForeignKey(s => s.PlanSportifId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relation Many-to-Many avec Coaches via CoachPlanSportif
                entity.HasMany(p => p.CoachPlanSportifs)
                      .WithOne(cps => cps.PlanSportif)
                      .HasForeignKey(cps => cps.PlanSportifId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Configuration des propriétés
                entity.Property(p => p.Nom)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Description)
                    .HasMaxLength(1000);

                entity.Property(p => p.Niveau)
                    .HasMaxLength(50);

                entity.Property(p => p.DureeSemaines)
                    .HasDefaultValue(4);
            });

            // ==================== CONFIGURATION COACH PLAN SPORTIF ====================
            modelBuilder.Entity<CoachPlanSportif>()
                .HasKey(cps => new { cps.CoachId, cps.PlanSportifId });

            modelBuilder.Entity<CoachPlanSportif>()
                .HasOne(cps => cps.Coach)
                .WithMany(c => c.CoachPlanSportifs)
                .HasForeignKey(cps => cps.CoachId);

            modelBuilder.Entity<CoachPlanSportif>()
                .HasOne(cps => cps.PlanSportif)
                .WithMany(p => p.CoachPlanSportifs)
                .HasForeignKey(cps => cps.PlanSportifId);

            // Configuration des propriétés supplémentaires
            modelBuilder.Entity<CoachPlanSportif>()
                .Property(cps => cps.DateAffectation)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<CoachPlanSportif>()
                .Property(cps => cps.Commentaire)
                .HasMaxLength(500);

            // ==================== CONFIGURATION SEANCE ====================
            modelBuilder.Entity<Seance>(entity =>
            {
                entity.HasKey(s => s.Id);

                // Relation Many-to-One avec PlanSportif
                entity.HasOne(s => s.PlanSportif)
                      .WithMany(p => p.Seances)
                      .HasForeignKey(s => s.PlanSportifId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Configuration des propriétés
                entity.Property(s => s.Nom)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(s => s.Description)
                    .HasMaxLength(1000);

                entity.Property(s => s.Jour)
                    .HasMaxLength(20);

                entity.Property(s => s.DureeMinutes)
                    .HasDefaultValue(60);

                entity.Property(s => s.Ordre)
                    .HasDefaultValue(1);
            });

            // ==================== CONFIGURATION EXERCICE ====================
            modelBuilder.Entity<Exercice>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Relation avec Seance
                entity.HasOne(e => e.Seance)
                      .WithMany(s => s.Exercices)
                      .HasForeignKey(e => e.SeanceId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Configuration des propriétés
                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.Instructions)
                    .HasMaxLength(2000);

                entity.Property(e => e.Materiel)
                    .HasMaxLength(500);

                entity.Property(e => e.Sets)
                    .HasDefaultValue(3);

                entity.Property(e => e.Repetitions)
                    .HasDefaultValue(10);

                entity.Property(e => e.DureeSecondes)
                    .HasDefaultValue(60);

                entity.Property(e => e.ReposSecondes)
                    .HasDefaultValue(30);
            });

            // ==================== CONFIGURATION PLAN NUTRITIONNEL ====================
            modelBuilder.Entity<PlanNutritionnel>(entity =>
            {
                entity.HasKey(p => p.Id);

                // Configuration des propriétés
                entity.Property(p => p.Nom)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Description)
                    .HasMaxLength(1000);

                entity.Property(p => p.TypeRegime)
                    .HasMaxLength(100);

                entity.Property(p => p.Objectif)
                    .HasMaxLength(200);

                entity.Property(p => p.CaloriesJournalieres)
                    .HasDefaultValue(2000);

                // Relation One-to-Many avec Aliments
                entity.HasMany(p => p.Aliments)
                      .WithOne(a => a.PlanNutritionnel)
                      .HasForeignKey(a => a.PlanNutritionnelId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ==================== CONFIGURATION ALIMENT ====================
            modelBuilder.Entity<Aliment>(entity =>
            {
                entity.HasKey(a => a.Id);

                // Configuration des propriétés
                entity.Property(a => a.Nom)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(a => a.Type)
                    .HasMaxLength(100);

                entity.Property(a => a.Calories)
                    .HasColumnType("decimal(6,2)");

                entity.Property(a => a.Proteines)
                    .HasColumnType("decimal(6,2)");

                entity.Property(a => a.Glucides)
                    .HasColumnType("decimal(6,2)");

                entity.Property(a => a.Lipides)
                    .HasColumnType("decimal(6,2)");

                entity.Property(a => a.Portion)
                    .HasMaxLength(100);

                entity.Property(a => a.MomentConsommation)
                    .HasMaxLength(100);
            });

            // ==================== CONFIGURATION SUIVI POIDS ====================
            modelBuilder.Entity<SuiviPoids>(entity =>
            {
                entity.HasKey(s => s.Id);

                // Configuration des propriétés
                entity.Property(s => s.Poids)
                    .IsRequired()
                    .HasColumnType("decimal(5,2)");

                // AJOUT DE LA PROPRIÉTÉ Date (en plus de DateMesure si nécessaire)
                entity.Property(s => s.Date) // AJOUTÉ
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(s => s.DateMesure)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(s => s.Commentaire)
                    .HasMaxLength(500);

                // Relation avec Client
                entity.HasOne(s => s.Client)
                      .WithMany(c => c.SuiviPoids)
                      .HasForeignKey(s => s.ClientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ==================== CONFIGURATION RAPPORT ====================
            modelBuilder.Entity<Rapport>(entity =>
            {
                entity.HasKey(r => r.Id);

                // Configuration des propriétés
                entity.Property(r => r.Titre)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(r => r.Contenu)
                    .HasMaxLength(4000);

                entity.Property(r => r.DateCreation)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(r => r.Type)
                    .HasMaxLength(100);

                // Relation avec Client
                entity.HasOne(r => r.Client)
                      .WithMany(c => c.Rapports)
                      .HasForeignKey(r => r.ClientId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relation avec Coach
                entity.HasOne(r => r.Coach)
                      .WithMany()
                      .HasForeignKey(r => r.CoachId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ==================== CONFIGURATION NOTIFICATION ====================
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);

                // Configuration des propriétés
                entity.Property(n => n.Titre)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(n => n.Message)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(n => n.DateCreation)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(n => n.Type)
                    .HasMaxLength(100);

                entity.Property(n => n.EstLue)
                    .HasDefaultValue(false);

                // Relation avec Utilisateur
                entity.HasOne(n => n.Utilisateur)
                      .WithMany()
                      .HasForeignKey(n => n.UtilisateurId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}