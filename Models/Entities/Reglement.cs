using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementMVC.Models.Entities
{
    public class Reglement
    {
        [Key]
        public int IdReglement { get; set; }

        [Required]
        [ForeignKey("Partenaire")]
        public int IdUser { get; set; }

        [Required]
        [Display(Name = "Date de paiement")]
        public DateTime DatePaiement { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Montant")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le montant doit être supérieur à 0")]
        public decimal Montant { get; set; }

        [Required]
        [Display(Name = "Mode de paiement")]
        public ModePaiement ModePaiement { get; set; }

        [Display(Name = "Numéro de paiement")]
        [StringLength(100)]
        public string? NumeroPaiement { get; set; }

        [Display(Name = "Note/Remarque")]
        [StringLength(500)]
        public string? Note { get; set; }

        [Display(Name = "Date de création")]
        public DateTime DateCreation { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual User Partenaire { get; set; }
    }
}
