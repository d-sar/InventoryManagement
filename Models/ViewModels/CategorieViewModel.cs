namespace InventoryManagementMVC.Models.ViewModels
{
    public class CategorieViewModel
    {
        public int IdCategorie { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NombreProduits { get; set; }
        public DateTime DateCreation { get; set; }
    }
}
