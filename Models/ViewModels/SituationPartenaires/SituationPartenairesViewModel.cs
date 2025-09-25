namespace InventoryManagementMVC.Models.ViewModels.SituationPartenaires
{
    public class SituationPartenairesViewModel
    {
        public List<PartenaireSelectViewModel> Partenaires { get; set; } = new List<PartenaireSelectViewModel>();
        public SituationFilterViewModel Filter { get; set; } = new SituationFilterViewModel();
        public SituationResultViewModel? Result { get; set; }
        //le montant a payer et le montant payé
        public decimal TotalDocuments { get; set; }     // Somme des documents (à payer)
        public decimal TotalReglements { get; set; }    // Montant payé
        public decimal ResteAPayer => TotalDocuments - TotalReglements;

    }
}
