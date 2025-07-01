using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.ViewModels
{
    public class ClientViewModel : UserViewModel
    {
        [Display(Name = "Total des achats")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal TotalAchats { get; set; }

        [Display(Name = "Date d'inscription")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime DateInscription { get; set; }

        [Display(Name = "Statut")]
        public bool EstActif { get; set; }

        [Display(Name = "Dernier achat")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DernierAchat { get; set; }

    }
}
