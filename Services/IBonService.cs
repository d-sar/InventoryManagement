using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;

namespace InventoryManagementMVC.Services
{
    public interface IBonService
    {
        Task<Bon> CreateBonAsync(string typeBon, int partenaireId, List<LigneBon> lignes);
        Task<Bon?> GetBonByIdAsync(int id);
        Task<List<Bon>> GetBonsByTypeAsync(string type);
        Task<List<Bon>> GetBonsByTypeAndFiltersAsync(string type, BonFilterViewModel filters);
        Task UpdateStockFromBonAsync(int bonId, string typeBon);
        Task<bool> DeleteBonAsync(int id);
        Task<List<Bon>> GetBonsByPartenaireAsync(int partenaireId, string type);
        Task<List<Bon>> GetBonsByDateRangeAsync(DateTime dateDebut, DateTime dateFin, string type);
        Task<BonStatsViewModel> GetBonStatsAsync(string type);
    }
}
