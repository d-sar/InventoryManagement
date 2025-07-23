namespace InventoryManagementMVC.Models.ViewModels.SituationPartenaires
{
    public class DocumentSituationViewModel
    {
        public DateTime Date { get; set; }
        public string NumeroDocument { get; set; } = string.Empty;
        public string TypeDocument { get; set; } = string.Empty;
        public string TitreDocument { get; set; } = string.Empty;
        public decimal Montant { get; set; }
        public bool EstPositif { get; set; }
    }
}
