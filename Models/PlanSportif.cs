using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_Manager.Models
{
    public class PlanSportif
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nom { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public int? DureeSemaines { get; set; }
        public string? Niveau { get; set; }

        // Relations
        public virtual ICollection<Seance> Seances { get; set; } = new List<Seance>();
        public virtual ICollection<CoachPlanSportif> CoachPlanSportifs { get; set; } = new List<CoachPlanSportif>();
    }
}