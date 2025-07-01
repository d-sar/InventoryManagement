using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class EditProduitViewModel
    {
        [Required]
        public int IdProduit {  get; set; }

        [Required(ErrorMessage = "Le libellé est requis.")]
        [StringLength(100, ErrorMessage = "Le libellé ne peut pas dépasser 100 caractères.")]
        [Display(Name = "Libellé")]
        public string? Libelle { get; set; }

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "La catégorie est requise.")]
        [Display(Name = "Catégorie")]
        public int? IdCategorie { get; set; }

        [Required(ErrorMessage = "Le prix unitaire est requis.")]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être supérieur ou égal à zéro.")]
        [Display(Name = "Prix Unitaire")]
        public decimal PrixUnitaire { get; set; }

        [Required(ErrorMessage = "La quantité en stock est requise.")]
        [Range(0, int.MaxValue, ErrorMessage = "La quantité doit être supérieure ou égale à zéro.")]
        [Display(Name = "Quantité en Stock")]
        public int QuantiteStock { get; set; }


    }
}
