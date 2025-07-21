namespace InventoryManagementMVC.Models.ViewModels
{
    public class StockGlobalViewModel
    {
        public int IdProduit { get; set; }
        public string Libelle { get; set; }
        public string Categorie { get; set; }
        public int SumEntrees { get; set; }  // BE + RC
        public int SumSorties { get; set; }  // BS + RF
        public int Stock { get; set; }       // SumEntrees - SumSorties
        public int StockPhysique { get; set; } // Stock actuel en base
        public int EntreesBE { get; set; }
        public int RetourClient { get; set; }
        public int SortiesBS { get; set; }
        public int RetourFournisseur { get; set; }
        public DateTime DateCalcul { get; set; }
        
    }
}
