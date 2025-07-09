using InventoryManagementMVC.Models.Entities;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class StockHistoryFilterViewModel
    {
        public int? ProduitId { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public List<Produit> Produits { get; set; } = new List<Produit>();
        public List<MouvementStockViewModel> Mouvements { get; set; } = new List<MouvementStockViewModel>();
        public string NomProduitSelectionne { get; set; }
        public int StockInitial { get; set; }
        public int StockFinal { get; set; }
        public List<DebugMouvementViewModel> DebugMouvements { get; set; } = new();

    }
}
