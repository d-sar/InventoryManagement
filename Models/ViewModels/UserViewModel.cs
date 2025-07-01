using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        [Display(Name = "Nom")]
        public string Nom { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères")]
        [Display(Name = "Adresse")]
        public string Adresse { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Le téléphone ne peut pas dépasser 20 caractères")]
        [Display(Name = "Téléphone")]
        public string Telephone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Adresse email invalide")]
        [StringLength(100, ErrorMessage = "L'email ne peut pas dépasser 100 caractères")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        
        [Display(Name = "Type")]
        public string Type { get; set; } = string.Empty;

        [Display(Name = "Nombre de bons")]
        public int NombreBons { get; set; }

       
    }
}
