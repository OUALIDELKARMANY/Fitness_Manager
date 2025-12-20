using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_Manager.Models
{
    public class Exercice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nom { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public int? Sets { get; set; }
        public int? Repetitions { get; set; }
        public int? DureeSecondes { get; set; }
        public int? ReposSecondes { get; set; }
        public string? Instructions { get; set; }
        public string? Materiel { get; set; }

        // Relations
        public int? SeanceId { get; set; }
        public virtual Seance? Seance { get; set; }
    }
}