using InventoryManagementMVC.Models.Entities;

namespace InventoryManagementMVC.Models.ViewModels
{

    public class StockHistoryViewModel
    {
        public Produit Produit { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public int StockInitial { get; set; }
        public List<LigneBon> Mouvements { get; set; }
    }
}
