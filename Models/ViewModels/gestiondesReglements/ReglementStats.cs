using InventoryManagementMVC.Models.Entities;

namespace InventoryManagementMVC.Models.ViewModels.gestiondesReglements
{
    public class ReglementStats
    {
        public int NombreReglements { get; set; }
        public decimal MontantTotal { get; set; }
        public decimal MoyenneParReglement { get; set; }
        public Dictionary<ModePaiement, decimal> RepartitionParMode { get; set; } = new Dictionary<ModePaiement, decimal>();

    }
}
