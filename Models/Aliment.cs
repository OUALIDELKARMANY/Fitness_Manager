using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_Manager.Models
{
    public class Aliment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nom { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Type { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? Calories { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? Proteines { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? Glucides { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? Lipides { get; set; }

        public string? Portion { get; set; }
        public string? MomentConsommation { get; set; }

        // Relations
        public int? PlanNutritionnelId { get; set; }
        public virtual PlanNutritionnel? PlanNutritionnel { get; set; }
    }
}