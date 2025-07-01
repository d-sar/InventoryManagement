using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class FournisseurViewModel : UserViewModel
    {
        [Display(Name = "Total des achats")]
        public decimal TotalFournitures { get; set; }

        [Display(Name = "Nombre de livraisons")]
        public int NombreLivraisons { get; set; }
    }
}
