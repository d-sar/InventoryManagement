using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class LigneBonViewModel
    {
        [Required(ErrorMessage = "Veuillez sélectionner un produit")]
        [Display(Name = "Produit")]
        public int IdProduit { get; set; }

        [Required(ErrorMessage = "La quantité est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être positive")]
        [Display(Name = "Quantité")]
        public int Quantite { get; set; }

        [Required(ErrorMessage = "Le prix unitaire est obligatoire")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le prix unitaire doit être positif")]
        [Display(Name = "Prix unitaire")]
        public decimal PrixUnitaire { get; set; }

        // Propriété calculée pour l'affichage
        public decimal Total => Quantite * PrixUnitaire;
    }
}
