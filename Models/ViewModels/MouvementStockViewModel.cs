namespace InventoryManagementMVC.Models.ViewModels
{
    public class MouvementStockViewModel
    {
        public string Article { get; set; }
        public int StockInitial { get; set; }
        public int NumeroDocument { get; set; }
        public DateTime Date { get; set; }

        // Colonnes séparées pour chaque type d'entrée/sortie
        public int? EntreesBE { get; set; }  // Bon d'Entrée
        public int? EntreesRF { get; set; }  // Retour Fournisseur
        public int? SortiesBS { get; set; }  // Bon de Sortie
        public int? SortiesRC { get; set; }  // Retour Client

        public int Stock { get; set; }
        public int StockFinal { get; set; }
        public string TypeDocument { get; set; }
    }
}
