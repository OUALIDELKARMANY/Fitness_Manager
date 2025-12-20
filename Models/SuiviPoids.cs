using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_Manager.Models
{
    public class SuiviPoids
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Poids { get; set; }

        public DateTime Date { get; set; } = DateTime.Now; // Renommez DateMesure en Date ou gardez DateMesure
        public DateTime DateMesure { get; set; } = DateTime.Now; // Ajoutez cette propriété

        [StringLength(500)]
        public string? Commentaire { get; set; }

        // Relations
        public int ClientId { get; set; }
        public virtual Client Client { get; set; } = null!;
    }
}