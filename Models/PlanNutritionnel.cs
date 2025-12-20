using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_Manager.Models
{
    public class PlanNutritionnel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nom { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public int? CaloriesJournalieres { get; set; }
        public string? TypeRegime { get; set; }
        public string? Objectif { get; set; }

        // Relations
        public virtual ICollection<Aliment> Aliments { get; set; } = new List<Aliment>();
    }
}