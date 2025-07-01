using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementMVC.Models.Entities
{
    public class Bon
    {
        [Key]
        public int IdBon { get; set; }

        [Required]
        [ForeignKey("User")]
        public int IdUser { get; set; }
        [Required]
        [ForeignKey("DocType")]
        public int IdDocType { get; set; }

        [Required]
        public int Numero { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        public User Partenaire { get; set; } = null!;
        public DocType DocType { get; set; } = null!;
        public virtual ICollection<LigneBon> LignesBon { get; set; } = new List<LigneBon>();

        public virtual decimal CalculerTotal()
        {
            return LignesBon.Sum(l => l.Quantite * l.PrixUnitaire);
        }

    }
}

