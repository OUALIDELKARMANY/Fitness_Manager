using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_Manager.Models
{
    public class Seance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nom { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public int? Ordre { get; set; }
        public string? Jour { get; set; }
        public int? DureeMinutes { get; set; }

        // Relations
        public int PlanSportifId { get; set; }
        public virtual PlanSportif PlanSportif { get; set; } = null!;

        public virtual ICollection<Exercice> Exercices { get; set; } = new List<Exercice>();
    }
}