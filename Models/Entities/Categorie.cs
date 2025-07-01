using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.Entities
{
    public class Categorie
    {
        [Key]
        public int IdCategorie { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<Produit> Produits { get; set; } = new List<Produit>();


    }
}
