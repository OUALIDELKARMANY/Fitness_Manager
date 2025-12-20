using System.ComponentModel.DataAnnotations;

namespace Fitness_Manager.Models
{
    public class Client : Utilisateur
    {
        // Date d'inscription spécifique au client
        public DateTime DateInscription { get; set; } = DateTime.Now;

        // Propriétés démographiques
        public int? Age { get; set; } // Ajoutez cette propriété

        // Propriétés physiques
        public decimal? Poids { get; set; }
        public decimal? PoidsActuel { get; set; } // Ajoutez cette propriété
        public decimal? Taille { get; set; }
        public string? Objectif { get; set; }

        // Fréquence d'entraînement
        public int? FrequenceEntrainement { get; set; }

        // Relations
        public int? CoachId { get; set; }
        public virtual Coach? Coach { get; set; }

        public int? PlanSportifId { get; set; }
        public virtual PlanSportif? PlanSportif { get; set; }

        public int? PlanNutritionnelId { get; set; }
        public virtual PlanNutritionnel? PlanNutritionnel { get; set; }

        // Collections
        public virtual ICollection<SuiviPoids> SuiviPoids { get; set; } = new List<SuiviPoids>();
        public virtual ICollection<Rapport> Rapports { get; set; } = new List<Rapport>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}