using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class BonCreateViewModel
    {
        [Required(ErrorMessage = "Veuillez sélectionner un partenaire")]
        [Display(Name = "Partenaire")]
        public int PartenaireId { get; set; }

        [Required]
        public string TypeBon { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "Le bon doit contenir au moins une ligne")]
        public List<LigneBonViewModel> Lignes { get; set; } = new List<LigneBonViewModel>();

        [Display(Name = "Date du Bon")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }


    }
}
