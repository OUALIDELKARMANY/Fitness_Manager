using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_Manager.Models
{
    [Table("Utilisateurs")]
    public class Utilisateur
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        public string? Sexe { get; set; } // Ajoutez cette propriété

        public string? Photo { get; set; } // Ajoutez cette propriété

        public DateTime DateCreation { get; set; } = DateTime.Now;
    }
}