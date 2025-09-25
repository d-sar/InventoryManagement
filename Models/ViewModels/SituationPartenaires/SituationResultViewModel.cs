namespace InventoryManagementMVC.Models.ViewModels.SituationPartenaires
{
    public class SituationResultViewModel
    {
        public PartenaireSelectViewModel Partenaire { get; set; } = new PartenaireSelectViewModel();
        public List<DocumentSituationViewModel> Documents { get; set; } = new List<DocumentSituationViewModel>();
        public decimal TotalGeneral { get; set; }
        public decimal TotalDocuments { get; set; }     // Somme des documents (à payer)
        public decimal TotalReglements { get; set; }    // Montant payé
        public decimal ResteAPayer => TotalGeneral - TotalReglements;

        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
    }
}
