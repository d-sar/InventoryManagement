using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementMVC.Models.Entities
{
    public class Produit
    {
        [Key]
        public int IdProduit { get; set; }

        [Required]
        [ForeignKey("Categorie")]
        public int IdCategorie { get; set; }

        [Required]
        [StringLength(200)]
        public string Libelle { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrixUnitaire { get; set; }

        [Required]
        public int QuantiteStock { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Categorie Categorie { get; set; } = null!;
        public virtual ICollection<LigneBon> LignesBon { get; set; } = new List<LigneBon>();
    }
}
