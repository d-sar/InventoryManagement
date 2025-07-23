using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.ViewModels.SituationPartenaires;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Services
{
    public class SituationPartenairesService : ISituationPartenairesService
    {
        private readonly ApplicationDbContext _context;

        public SituationPartenairesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PartenaireSelectViewModel>> GetAllPartenairesAsync()
        {
            return await _context.User
                .Select(u => new PartenaireSelectViewModel
                {
                    Id = u.Id,
                    Nom = u.Nom,
                    Type = u.Type,
                    Adresse = u.Adresse,
                    Email = u.Email
                })
                .OrderBy(p => p.Type)
                .ThenBy(p => p.Nom)
                .ToListAsync();
        }

        public async Task<List<PartenaireSelectViewModel>> GetPartenairesByTypeAsync(string type)
        {
            return await _context.User
                .Where(u => u.Type == type)
                .Select(u => new PartenaireSelectViewModel
                {
                    Id = u.Id,
                    Nom = u.Nom,
                    Type = u.Type,
                    Adresse = u.Adresse,
                    Email = u.Email
                })
                .OrderBy(p => p.Nom)
                .ToListAsync();
        }

        public async Task<SituationResultViewModel?> GetSituationPartenaireAsync(int partenaireId, DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            var partenaire = await _context.User
                .Where(u => u.Id == partenaireId)
                .Select(u => new PartenaireSelectViewModel
                {
                    Id = u.Id,
                    Nom = u.Nom,
                    Type = u.Type,
                    Adresse = u.Adresse,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();

            if (partenaire == null)
                return null;

            // Requête pour récupérer les bons avec filtres de date
            var bonsQuery = _context.Bons
                .Include(b => b.DocType)
                .Include(b => b.LignesBon)
                .Where(b => b.IdUser == partenaireId);

            if (dateDebut.HasValue)
                bonsQuery = bonsQuery.Where(b => b.Date >= dateDebut.Value);

            if (dateFin.HasValue)
                bonsQuery = bonsQuery.Where(b => b.Date <= dateFin.Value);

            var bons = await bonsQuery
                .OrderBy(b => b.Date)
                .ToListAsync();

            var documents = bons.Select(b => new DocumentSituationViewModel
            {
                Date = b.Date,
                NumeroDocument = GetNumeroDocument(b.DocType.Type, b.Numero),
                TypeDocument = b.DocType.Type,
                TitreDocument = b.DocType.Titre,
                Montant = b.CalculerTotal(),
                EstPositif = DeterminerSiPositif(b.DocType.Type, partenaire.Type)
            }).ToList();

            // Calculer le total en fonction du type de document et partenaire
            var totalGeneral = CalculerTotalGeneral(documents);

            return new SituationResultViewModel
            {
                Partenaire = partenaire,
                Documents = documents,
                TotalGeneral = totalGeneral,
                DateDebut = dateDebut ?? DateTime.MinValue,
                DateFin = dateFin ?? DateTime.MaxValue
            };
        }

        private string GetNumeroDocument(string typeDoc, int numero)
        {
            return typeDoc switch
            {
                "BonSortie" => $"BS {numero:D4}",
                "BonEntree" => $"BE {numero:D4}",
                "RetourClient" => $"RC {numero:D4}",
                "RetourFournisseur" => $"RF {numero:D4}",
                
                _ => $"{typeDoc} {numero:D4}"
            };
        }

        private bool DeterminerSiPositif(string typeDocument, string typePartenaire)
        {
            return (typeDocument, typePartenaire) switch
            {
                // Pour les clients
                ("BonSortie", "Client") => true,      // Vente au client = positif
                ("RetourClient", "Client") => false,  // Retour du client = négatif
                ("Facture", "Client") => true,        // Facture client = positif

                // Pour les fournisseurs
                ("BonEntree", "Fournisseur") => false,     // Achat au fournisseur = négatif
                ("RetourFournisseur", "Fournisseur") => true, // Retour au fournisseur = positif
                ("Facture", "Fournisseur") => false,       // Facture fournisseur = négatif

                _ => true // Par défaut positif
            };
        }

        private decimal CalculerTotalGeneral(List<DocumentSituationViewModel> documents)
        {
            return documents.Sum(d => d.EstPositif ? d.Montant : -d.Montant);
        }
    }
}
