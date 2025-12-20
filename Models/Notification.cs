using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_Manager.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titre { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; } = DateTime.Now;
        public bool EstLue { get; set; } = false;
        public string? Type { get; set; }

        // Relations
        public int UtilisateurId { get; set; }
        public virtual Utilisateur Utilisateur { get; set; } = null!;
    }
}