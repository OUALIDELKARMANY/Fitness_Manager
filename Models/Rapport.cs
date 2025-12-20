using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_Manager.Models
{
    public class Rapport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titre { get; set; } = string.Empty;

        [StringLength(4000)]
        public string? Contenu { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;
        public string? Type { get; set; }

        // Relations
        public int ClientId { get; set; }
        public virtual Client Client { get; set; } = null!;

        public int? CoachId { get; set; }
        public virtual Coach? Coach { get; set; }
    }
}