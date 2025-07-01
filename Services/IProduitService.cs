using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;

namespace InventoryManagementMVC.Services
{
    public interface IProduitService

    {
        Task<IEnumerable<Produit>> GetAllProduitsAsync();
        Task<Produit?> GetProduitByIdAsync(int id);
        Task<Produit> CreateProduitAsync(CreateProduitViewModel model);
        Task<bool> UpdateProduitAsync(int id, CreateProduitViewModel model);
        Task<bool> DeleteProduitAsync(int id);
        Task<IEnumerable<Produit>> SearchProduitsAsync(string searchTerm);
        Task<IEnumerable<Produit>> GetProduitsByCategorieAsync(int categorieId);
        Task<bool> UpdateStockAsync(int produitId, int nouvelleQuantite);
    }
}