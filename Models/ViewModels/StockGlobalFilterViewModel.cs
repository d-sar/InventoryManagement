using InventoryManagementMVC.Models.Entities;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class StockGlobalFilterViewModel
    {
        public DateTime? DateLimite { get; set; }
        public int? CategorieId { get; set; }
        public string RechercheLibelle { get; set; }
        public List<StockGlobalViewModel> Stocks { get; set; } = new List<StockGlobalViewModel>();
        public List<Categorie> Categories { get; set; } = new List<Categorie>();
        public int TotalProduits { get; set; }
        public int TotalStock { get; set; }
        public int TotalEntrees { get; set; }
        public int TotalSorties { get; set; }
    }
}
