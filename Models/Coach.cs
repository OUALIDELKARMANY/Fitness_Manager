// Models/Coach.cs
using System.ComponentModel.DataAnnotations;

namespace Fitness_Manager.Models
{
    public class Coach : Utilisateur
    {
        // Spécialités du coach
        public string? Specialite { get; set; }

        [Range(0, 50)]
        public int? AnneesExperience { get; set; }

        public string? Certifications { get; set; }

        // Disponibilités
        public string? Disponibilites { get; set; }

        // Tarification
        [Range(0, 1000)]
        public decimal? TarifHoraire { get; set; }

        // Relations
        public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
        public virtual ICollection<CoachPlanSportif> CoachPlanSportifs { get; set; } = new List<CoachPlanSportif>();
    }
}