using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementMVC.Models.Entities
{
    public enum ModePaiement
    {
        [Display(Name = "Espèces")]
        Especes = 1,
        
        [Display(Name = "Chèque")]
        Cheque = 2,
        
        [Display(Name = "Effet")]
        Effet = 3,
        
        [Display(Name = "Virement bancaire")]
        VirementBancaire = 4
    }
}
