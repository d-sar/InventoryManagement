using InventoryManagementMVC.Models.Entities;

namespace InventoryManagementMVC.Services
{
    public interface IBonEntreeService
    {
        Task<Bon> CreateBonEntreeAsync(int fournisseurId, List<LigneBon> lignes);
        Task<Bon?> GetBonEntreeByIdAsync(int id);
        Task<List<Bon>> GetAllBonsEntreeAsync();
        Task UpdateStockFromBonEntreeAsync(int bonId);
        Task<bool> DeleteBonEntreeAsync(int id);
        Task<List<Bon>> GetBonsEntreeByFournisseurAsync(int fournisseurId);
        Task<List<Bon>> GetBonsEntreeByDateRangeAsync(DateTime dateDebut, DateTime dateFin);

    }
}
