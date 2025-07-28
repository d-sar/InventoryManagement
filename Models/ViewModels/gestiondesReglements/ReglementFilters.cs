using InventoryManagementMVC.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels.gestiondesReglements
{
    public class ReglementFilters
    {

        [Display(Name = "Partenaire")]
        public int? PartenaireId { get; set; }

        [Display(Name = "Date début")]
        [DataType(DataType.Date)]
        public DateTime? DateDebut { get; set; }

        [Display(Name = "Date fin")]
        [DataType(DataType.Date)]
        public DateTime? DateFin { get; set; }

        [Display(Name = "Mode de paiement")]
        public ModePaiement? ModePaiement { get; set; }

        [Display(Name = "Montant minimum")]
        public decimal? MontantMin { get; set; }

        [Display(Name = "Montant maximum")]
        public decimal? MontantMax { get; set; }
    }
}
