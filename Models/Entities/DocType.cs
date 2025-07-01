using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Models.Entities
{
    public class DocType
    {
        [Key]
        public int IdDocType { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Titre { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Bon> Bons { get; set; } = new List<Bon>();
    }
}
