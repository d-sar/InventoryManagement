using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;

namespace InventoryManagementMVC.Services
{
    public interface ICategorieService
    {
        Task<IEnumerable<Categorie>> GetAllCategoriesAsync();
        Task<Categorie?> GetCategorieByIdAsync(int id);
        Task<Categorie> CreateCategorieAsync(CreateCategorieViewModel model);
        Task<bool> UpdateCategorieAsync(int id, CreateCategorieViewModel model);
        Task<bool> DeleteCategorieAsync(int id);
        Task<IEnumerable<Categorie>> SearchCategoriesAsync(string searchTerm);
        Task<bool> CategorieHasProduitsAsync(int categorieId);
        Task<int> GetProduitsCountByCategorieAsync(int categorieId);

    }
}
