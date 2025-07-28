using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels.gestiondesReglements;

namespace InventoryManagementMVC.Services
{
    public interface IReglementService
    {
        Task<ReglementIndexViewModel> GetReglementIndexViewModelAsync(ReglementFilters filters);
        Task<ReglementCreateEditViewModel> GetCreateViewModelAsync();
        Task<ReglementCreateEditViewModel> GetEditViewModelAsync(int id);
        Task<ReglementDetailsViewModel> GetDetailsViewModelAsync(int id);
        Task<bool> CreateReglementAsync(Reglement reglement);
        Task<bool> UpdateReglementAsync(Reglement reglement);
        Task<bool> DeleteReglementAsync(int id);
        Task<Reglement?> GetReglementByIdAsync(int id);
        Task<List<Reglement>> GetReglementsByPartenaireAsync(int partenaireId);
        Task<ReglementStats> GetStatsAsync(ReglementFilters filters);
    }
}
