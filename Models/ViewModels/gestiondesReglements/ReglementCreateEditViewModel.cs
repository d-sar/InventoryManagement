using InventoryManagementMVC.Models.Entities;

namespace InventoryManagementMVC.Models.ViewModels.gestiondesReglements
{
    public class ReglementCreateEditViewModel
    {
        public Reglement Reglement { get; set; } = new Reglement();
        public List<User> Partenaires { get; set; } = new List<User>();
        public string TypePartenaire { get; set; } = "Partenaire";
        public string ActionTitle { get; set; } = "Nouveau règlement";
        public bool IsEdit { get; set; } = false;
    }
}
