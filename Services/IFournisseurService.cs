using InventoryManagementMVC.Models.Entities;

namespace InventoryManagementMVC.Services
{
    public interface IFournisseurService
    {
        Task<IEnumerable<Fournisseur>> GetAllFournisseursAsync();
        Task<Fournisseur?> GetFournisseurByIdAsync(int id);
        Task AddFournisseurAsync(Fournisseur fournisseur);
        Task UpdateFournisseurAsync(Fournisseur fournisseur);
        Task DeleteFournisseurAsync(int id);
    }
}
