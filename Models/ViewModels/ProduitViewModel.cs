namespace InventoryManagementMVC.Models.ViewModels
{
    public class ProduitViewModel
    {
        public int IdProduit { get; set; }
        public string Libelle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PrixUnitaire { get; set; }
        public int QuantiteStock { get; set; }
        public DateTime DateCreation { get; set; }
        public string NomCategorie { get; set; } = string.Empty;
        public string StatutStock => QuantiteStock <= 10 ? "Stock faible" : "Stock normal";

    }
}
