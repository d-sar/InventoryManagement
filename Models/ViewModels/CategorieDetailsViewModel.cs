namespace InventoryManagementMVC.Models.ViewModels
{
    public class CategorieDetailsViewModel
    {
        public int IdCategorie { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NombreProduits { get; set; }
        public DateTime DateCreation { get; set; }
        public List<ProduitViewModel> Produits { get; set; } = new List<ProduitViewModel>();
    }
}
