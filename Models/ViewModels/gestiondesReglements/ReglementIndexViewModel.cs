using InventoryManagementMVC.Models.Entities;

namespace InventoryManagementMVC.Models.ViewModels.gestiondesReglements
{
    public class ReglementIndexViewModel
    {
        public List<Reglement> Reglements { get; set; } = new List<Reglement>();
        public List<User> Partenaires { get; set; } = new List<User>();
        public string TypePartenaire { get; set; } = "Partenaire";
        public string TitrePage { get; set; } = "Règlements";
        public ReglementFilters Filters { get; set; } = new ReglementFilters();
        public ReglementStats Stats { get; set; } = new ReglementStats();
    }
}
