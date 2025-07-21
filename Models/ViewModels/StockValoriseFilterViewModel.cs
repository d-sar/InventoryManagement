using InventoryManagementMVC.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class StockValoriseFilterViewModel
    {
        [Display(Name = "Date Limite")]
        public DateTime? DateLimite { get; set; }

        [Display(Name = "Catégorie")]
        public int? CategorieId { get; set; }

        [Display(Name = "Recherche Produit")]
        public string RechercheLibelle { get; set; }

        public List<StockValoriseViewModel> Stocks { get; set; } = new List<StockValoriseViewModel>();
        public List<Categorie> Categories { get; set; } = new List<Categorie>();

        // Statistiques globales
        public int TotalProduits { get; set; }
        public int TotalQuantiteEntrees { get; set; }
        public int TotalQuantiteSorties { get; set; }
        public int TotalStockPhysique { get; set; }
        public decimal TotalStockValorise { get; set; }
        public decimal PmpGlobal { get; set; }
    }
}
