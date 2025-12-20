using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_Manager.Models
{
    [Table("CoachPlanSportifs")]
    public class CoachPlanSportif
    {
        public int CoachId { get; set; }
        public virtual Coach Coach { get; set; } = null!;

        public int PlanSportifId { get; set; }
        public virtual PlanSportif PlanSportif { get; set; } = null!;

        public DateTime DateAffectation { get; set; } = DateTime.Now;
        public string? Commentaire { get; set; }
    }
}