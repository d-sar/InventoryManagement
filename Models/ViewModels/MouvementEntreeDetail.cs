namespace InventoryManagementMVC.Models.ViewModels
{
    public class MouvementEntreeDetail
    {
        public int IdBon { get; set; }
        public string NumeroBon { get; set; }
        public DateTime DateBon { get; set; }
        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }
        public decimal MontantTotal { get; set; }
        public string TypeDoc { get; set; }
    }
}
