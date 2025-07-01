using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class CreateCategorieViewModel
    {
        [Required(ErrorMessage = "Le nom de la catégorie est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        [Display(Name = "Nom de la catégorie")]
        public string Nom { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;
    }
}
