using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.Entities
{
    public abstract class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [StringLength(200)]
        public string Adresse { get; set; } = string.Empty;

        [StringLength(20)]
        public string Telephone { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;
        public ICollection<Bon> Bons { get; set; } = null!;
        public ICollection<Reglement> Reglements { get; set; } = null!;


        public virtual void CreerBon()
        {
            // Logique commune pour créer un bon
        }
    }

}

