using InventoryManagementMVC.Models.Entities;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class BonIndexViewModel
    {
        public List<Bon> Bons { get; set; } = new List<Bon>();
        public List<User> Partenaires { get; set; } = new List<User>();
        public BonFilterViewModel Filters { get; set; } = new BonFilterViewModel();
        public BonStatsViewModel Stats { get; set; } = new BonStatsViewModel();
        public string TypeBon { get; set; } = string.Empty;
        public string TitrePage { get; set; } = string.Empty;
        public string TypePartenaire { get; set; } = string.Empty;
    }
}
