using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class CreateProduitViewModel
    {
        [Required(ErrorMessage = "La catégorie est obligatoire")]
        [Display(Name = "Catégorie")]
        public int IdCategorie { get; set; }

        [Required(ErrorMessage = "Le libellé est obligatoire")]
        [StringLength(200, ErrorMessage = "Le libellé ne peut pas dépasser 200 caractères")]
        [Display(Name = "Libellé")]
        public string Libelle { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prix unitaire est obligatoire")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le prix doit être supérieur à 0")]
        [Display(Name = "Prix unitaire")]
        public decimal PrixUnitaire { get; set; }

        [Required(ErrorMessage = "La quantité en stock est obligatoire")]
        [Range(0, int.MaxValue, ErrorMessage = "La quantité doit être positive")]
        [Display(Name = "Quantité en stock")]
        public int QuantiteStock { get; set; }
}
}
