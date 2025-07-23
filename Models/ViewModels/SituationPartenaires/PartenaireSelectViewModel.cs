namespace InventoryManagementMVC.Models.ViewModels.SituationPartenaires
{
    public class PartenaireSelectViewModel
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
