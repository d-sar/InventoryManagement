namespace InventoryManagementMVC.Models.ViewModels.SituationPartenaires
{
    public class SituationPartenairesViewModel
    {
        public List<PartenaireSelectViewModel> Partenaires { get; set; } = new List<PartenaireSelectViewModel>();
        public SituationFilterViewModel Filter { get; set; } = new SituationFilterViewModel();
        public SituationResultViewModel? Result { get; set; }
    }
}
