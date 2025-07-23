using InventoryManagementMVC.Models.ViewModels.SituationPartenaires;

namespace InventoryManagementMVC.Services
{
    public interface ISituationPartenairesService
    {
        Task<List<PartenaireSelectViewModel>> GetAllPartenairesAsync();
        Task<List<PartenaireSelectViewModel>> GetPartenairesByTypeAsync(string type);
        Task<SituationResultViewModel?> GetSituationPartenaireAsync(int partenaireId, DateTime? dateDebut = null, DateTime? dateFin = null);
    }
}
