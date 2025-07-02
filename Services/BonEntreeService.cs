using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace InventoryManagementMVC.Services
{
    public class BonEntreeService : IBonEntreeService
    {
        private readonly ApplicationDbContext _context;

        public BonEntreeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Bon> CreateBonEntreeAsync(int fournisseurId, List<LigneBon> lignes)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Vérifier que le fournisseur existe
                var fournisseur = await _context.Fournisseurs.FindAsync(fournisseurId);
                if (fournisseur == null)
                    throw new ArgumentException("Fournisseur introuvable");

                // Récupérer le type de document "Entrée"
                var docType = await _context.DocTypes.FirstOrDefaultAsync(d => d.Type == "Entree");
                if (docType == null)
                    throw new InvalidOperationException("Type de document 'Entrée' introuvable");

                // Générer le numéro de bon
                var derniereEntree = await _context.Bons
                    .Where(b => b.DocType.Type == "Entree")
                    .OrderByDescending(b => b.Numero)
                    .FirstOrDefaultAsync();

                int nouveauNumero = (derniereEntree?.Numero ?? 0) + 1;

                // Créer le bon d'entrée
                var bon = new Bon
                {
                    IdUser = fournisseurId,
                    IdDocType = docType.IdDocType,
                    Numero = nouveauNumero,
                    Date = DateTime.Now,
                    LignesBon = new List<LigneBon>()
                };

                // Ajouter les lignes
                foreach (var ligne in lignes)
                {
                    // Vérifier que le produit existe
                    var produit = await _context.Produits.FindAsync(ligne.IdProduit);
                    if (produit == null)
                        throw new ArgumentException($"Produit avec ID {ligne.IdProduit} introuvable");

                    if (ligne.Quantite <= 0)
                        throw new ArgumentException("La quantité doit être positive");

                    if (ligne.PrixUnitaire <= 0)
                        throw new ArgumentException("Le prix unitaire doit être positif");

                    bon.LignesBon.Add(new LigneBon
                    {
                        IdProduit = ligne.IdProduit,
                        Quantite = ligne.Quantite,
                        PrixUnitaire = ligne.PrixUnitaire
                    });
                }

                if (!bon.LignesBon.Any())
                    throw new ArgumentException("Le bon doit contenir au moins une ligne");

                _context.Bons.Add(bon);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return bon;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Bon?> GetBonEntreeByIdAsync(int id)
        {
            return await _context.Bons
                .Include(b => b.Partenaire)
                .Include(b => b.DocType)
                .Include(b => b.LignesBon)
                    .ThenInclude(l => l.Produit)
                        .ThenInclude(p => p.Categorie)
                .Where(b => b.DocType.Type == "Entree" && b.IdBon == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Bon>> GetAllBonsEntreeAsync()
        {
            return await _context.Bons
                .Include(b => b.Partenaire)
                .Include(b => b.DocType)
                .Include(b => b.LignesBon)
                    .ThenInclude(l => l.Produit)
                .Where(b => b.DocType.Type == "Entree")
                .OrderByDescending(b => b.Date)
                .ToListAsync();
        }

        public async Task UpdateStockFromBonEntreeAsync(int bonId)
        {
            var bon = await _context.Bons
                .Include(b => b.LignesBon)
                .Where(b => b.IdBon == bonId && b.DocType.Type == "Entree")
                .FirstOrDefaultAsync();

            if (bon == null)
                throw new ArgumentException("Bon d'entrée introuvable");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var ligne in bon.LignesBon)
                {
                    var produit = await _context.Produits.FindAsync(ligne.IdProduit);
                    if (produit != null)
                    {
                        produit.QuantiteStock += ligne.Quantite;
                        // Mettre à jour le prix unitaire du produit si nécessaire
                        produit.PrixUnitaire = ligne.PrixUnitaire;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteBonEntreeAsync(int id)
        {
            var bon = await _context.Bons
                .Include(b => b.LignesBon)
                .Where(b => b.IdBon == id && b.DocType.Type == "Entree")
                .FirstOrDefaultAsync();

            if (bon == null)
                return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Réduire le stock (annuler l'entrée)
                foreach (var ligne in bon.LignesBon)
                {
                    var produit = await _context.Produits.FindAsync(ligne.IdProduit);
                    if (produit != null)
                    {
                        produit.QuantiteStock -= ligne.Quantite;
                        if (produit.QuantiteStock < 0)
                            produit.QuantiteStock = 0;
                    }
                }

                _context.Bons.Remove(bon);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Bon>> GetBonsEntreeByFournisseurAsync(int fournisseurId)
        {
            return await _context.Bons
                .Include(b => b.Partenaire)
                .Include(b => b.DocType)
                .Include(b => b.LignesBon)
                    .ThenInclude(l => l.Produit)
                .Where(b => b.DocType.Type == "Entree" && b.IdUser == fournisseurId)
                .OrderByDescending(b => b.Date)
                .ToListAsync();
        }

        public async Task<List<Bon>> GetBonsEntreeByDateRangeAsync(DateTime dateDebut, DateTime dateFin)
        {
            return await _context.Bons
                .Include(b => b.Partenaire)
                .Include(b => b.DocType)
                .Include(b => b.LignesBon)
                    .ThenInclude(l => l.Produit)
                .Where(b => b.DocType.Type == "Entree" &&
                           b.Date >= dateDebut && b.Date <= dateFin)
                .OrderByDescending(b => b.Date)
                .ToListAsync();
        }
    }
}




