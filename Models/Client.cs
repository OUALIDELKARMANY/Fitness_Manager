using System.Collections.Generic;

namespace Fitness_Manager.Models
{
    public class Client : Utilisateur  
    {
        public int Age { get; set; }
        public double Taille { get; set; } 
        public double PoidsActuel { get; set; }

        public int CoachId { get; set; } // clé étrangère
        public Coach Coach { get; set; }

        public PlanSportif PlanSportif { get; set; } // Chaque client a un plan sportif
        public PlanNutritionnel PlanNutritionnel { get; set; }

        public List<SuiviPoids> SuiviPoids { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<Rapport> Rapports { get; set; }

    }
}
