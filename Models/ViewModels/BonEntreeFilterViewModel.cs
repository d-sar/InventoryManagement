using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class BonEntreeFilterViewModel
    {
        [Display(Name = "Fournisseur")]
        public int? FournisseurId { get; set; }

        [Display(Name = "Date début")]
        [DataType(DataType.Date)]
        public DateTime? DateDebut { get; set; }

        [Display(Name = "Date fin")]
        [DataType(DataType.Date)]
        public DateTime? DateFin { get; set; }

        [Display(Name = "Numéro de bon")]
        public int? Numero { get; set; }
    }
}
