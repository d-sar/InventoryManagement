using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels.SituationPartenaires
{
    public class SituationFilterViewModel
    {
        [Display(Name = "Type de partenaire")]
        public string? TypePartenaire { get; set; }

        [Display(Name = "Partenaire")]
        public int? PartenaireId { get; set; }

        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        public DateTime? DateDebut { get; set; }

        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        public DateTime? DateFin { get; set; }
    }
}
