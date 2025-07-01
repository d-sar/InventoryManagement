using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementMVC.Models.Entities
{
    public class LigneBon
    {
        [Key]
        public int IdLigne { get; set; }

        [Required]
        [ForeignKey("Bon")]
        public int IdBon { get; set; }

        [Required]
        [ForeignKey("Produit")]
        public int IdProduit { get; set; }

        [Required]
        public int Quantite { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrixUnitaire { get; set; }

       
        // Navigation properties
        public virtual Bon Bon { get; set; } = null!;
        public virtual Produit Produit { get; set; } = null!;
    }
}
