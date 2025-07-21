using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Services
{
    public class StockValoriseService
    {
        private readonly ApplicationDbContext _context;

        public StockValoriseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StockValoriseFilterViewModel> GetStockValorise(
            DateTime? dateLimite = null,
            int? categorieId = null,
            string rechercheLibelle = null)
        {
            var query = _context.Produits
                .Include(p => p.Categorie)
                .AsQueryable();

            // Appliquer les filtres
            if (categorieId.HasValue)
            {
                query = query.Where(p => p.IdCategorie == categorieId.Value);
            }

            if (!string.IsNullOrEmpty(rechercheLibelle))
            {
                query = query.Where(p => p.Libelle.Contains(rechercheLibelle));
            }

            var produits = await query.ToListAsync();
            var stocksValorise = new List<StockValoriseViewModel>();

            foreach (var produit in produits)
            {
                var stockValorise = await CalculerStockValorisePourProduit(produit, dateLimite);
                stocksValorise.Add(stockValorise);
            }

            // Récupérer toutes les catégories pour le filtre
            var categories = await _context.Categories.OrderBy(c => c.Nom).ToListAsync();

            // Calculer les totaux
            var result = new StockValoriseFilterViewModel
            {
                DateLimite = dateLimite,
                CategorieId = categorieId,
                RechercheLibelle = rechercheLibelle,
                Stocks = stocksValorise.OrderBy(s => s.Libelle).ToList(),
                Categories = categories,
                TotalProduits = stocksValorise.Count,
                TotalQuantiteEntrees = stocksValorise.Sum(s => s.SumQuantiteEntrees),
                TotalQuantiteSorties = stocksValorise.Sum(s => s.SumSorties),
                TotalStockPhysique = stocksValorise.Sum(s => s.StockPhysique),
                TotalStockValorise = stocksValorise.Sum(s => s.StockValorise)
            };

            // Calculer le PMP global
            if (result.TotalQuantiteEntrees > 0)
            {
                var totalMontantEntrees = stocksValorise.Sum(s => s.SumPrixUnitaire);
                result.PmpGlobal = totalMontantEntrees / result.TotalQuantiteEntrees;
            }

            return result;
        }

        private async Task<StockValoriseViewModel> CalculerStockValorisePourProduit(
            Produit produit,
            DateTime? dateLimite = null)
        {
            // Récupérer tous les mouvements d'entrée pour ce produit
            var mouvementsEntreeQuery = _context.LignesBon
                .Include(l => l.Bon)
                .ThenInclude(b => b.DocType)
                .Where(l => l.IdProduit == produit.IdProduit)
                .Where(l => l.Bon.DocType.Type == "BE" || l.Bon.DocType.Type == "ENTREE" ||
                           l.Bon.DocType.Type == "RC" || l.Bon.DocType.Type == "RETOURCLIENT");

            // Récupérer tous les mouvements de sortie pour ce produit
            var mouvementsSortieQuery = _context.LignesBon
                .Include(l => l.Bon)
                .ThenInclude(b => b.DocType)
                .Where(l => l.IdProduit == produit.IdProduit)
                .Where(l => l.Bon.DocType.Type == "BS" || l.Bon.DocType.Type == "SORTIE" ||
                           l.Bon.DocType.Type == "RF" || l.Bon.DocType.Type == "RETOURFOURNISSEUR");

            // Appliquer le filtre de date si spécifié
            if (dateLimite.HasValue)
            {
                mouvementsEntreeQuery = mouvementsEntreeQuery.Where(l => l.Bon.Date <= dateLimite.Value);
                mouvementsSortieQuery = mouvementsSortieQuery.Where(l => l.Bon.Date <= dateLimite.Value);
            }

            var mouvementsEntree = await mouvementsEntreeQuery.ToListAsync();
            var mouvementsSortie = await mouvementsSortieQuery.ToListAsync();

            // Calculer les détails des mouvements d'entrée
            var mouvementsEntreeDetails = mouvementsEntree.Select(m => new MouvementEntreeDetail
            {
                IdBon = m.IdBon,
                NumeroBon = m.Bon.Numero.ToString(),
                DateBon = m.Bon.Date,
                Quantite = m.Quantite,
                PrixUnitaire = m.PrixUnitaire,
                MontantTotal = m.Quantite * m.PrixUnitaire,
                TypeDoc = m.Bon.DocType.Type
            }).ToList();

            // Calculer les totaux
            int sumQuantiteEntrees = mouvementsEntree.Sum(m => m.Quantite);
            decimal sumMontantEntrees = mouvementsEntree.Sum(m => m.Quantite * m.PrixUnitaire);
            int sumQuantiteSorties = mouvementsSortie.Sum(m => m.Quantite);

            // Calculer le PMP (Prix Moyen Pondéré)
            decimal pmp = 0;
            if (sumQuantiteEntrees > 0)
            {
                pmp = sumMontantEntrees / sumQuantiteEntrees;
            }

            // Calculer le stock actuel (entrées - sorties)
            int stockActuel = sumQuantiteEntrees - sumQuantiteSorties;

            // Calculer le stock valorisé
            decimal stockValorise = stockActuel * pmp;

            return new StockValoriseViewModel
            {
                IdProduit = produit.IdProduit,
                Libelle = produit.Libelle,
                Categorie = produit.Categorie?.Nom ?? "Sans catégorie",
                SumQuantiteEntrees = sumQuantiteEntrees,
                SumPrixUnitaire = sumMontantEntrees,
                SumSorties = sumQuantiteSorties,
                PMP = pmp,
                StockValorise = stockValorise,
                StockPhysique = produit.QuantiteStock,
                DateCalcul = dateLimite ?? DateTime.Now,
                MouvementsEntrees = mouvementsEntreeDetails
            };
        }

        public async Task<List<Categorie>> GetAllCategories()
        {
            return await _context.Categories.OrderBy(c => c.Nom).ToListAsync();
        }

        public async Task<StockValoriseViewModel> GetDetailsProduit(int idProduit, DateTime? dateLimite = null)
        {
            var produit = await _context.Produits
                .Include(p => p.Categorie)
                .FirstOrDefaultAsync(p => p.IdProduit == idProduit);

            if (produit == null)
                throw new ArgumentException("Produit non trouvé");

            return await CalculerStockValorisePourProduit(produit, dateLimite);
        }
    }
}
