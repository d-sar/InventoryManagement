using InventoryManagementMVC.Models.Entities;

namespace InventoryManagementMVC.Models.ViewModels.gestiondesReglements
{
    public class ReglementDetailsViewModel
    {
        public Reglement Reglement { get; set; }
        public string TypePartenaire { get; set; } = "Partenaire";

    }
}
