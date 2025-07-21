using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class StockValoriseViewModel
    {
        public int IdProduit { get; set; }
        public string Libelle { get; set; }
        public string Categorie { get; set; }

        [Display(Name = "Sum DE Quantité Entrées")]
        public int SumQuantiteEntrees { get; set; }

        [Display(Name = "Sum de Prix Unitaire")]
        public decimal SumPrixUnitaire { get; set; }

        [Display(Name = "Sum de Sortie")]
        public int SumSorties { get; set; }

        [Display(Name = "PMP")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal PMP { get; set; }

        [Display(Name = "Stock Valorisé")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal StockValorise { get; set; }

        [Display(Name = "Stock Physique")]
        public int StockPhysique { get; set; }

        public DateTime DateCalcul { get; set; }

        // Détails pour le calcul
        public List<MouvementEntreeDetail> MouvementsEntrees { get; set; } = new List<MouvementEntreeDetail>();

    }
}
