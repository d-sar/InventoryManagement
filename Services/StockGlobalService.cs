using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Services
{
    public class StockGlobalService
    {
        private readonly ApplicationDbContext _context;

        public StockGlobalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StockGlobalFilterViewModel> GetStockGlobal(
            DateTime? dateLimite = null,
            int? categorieId = null,
            string rechercheLibelle = null)
        {
            var query = _context.Produits
                .Include(p => p.Categorie)
                .AsQueryable();

            // Filtres
            if (categorieId.HasValue)
            {
                query = query.Where(p => p.IdCategorie == categorieId.Value);
            }

            if (!string.IsNullOrEmpty(rechercheLibelle))
            {
                query = query.Where(p => p.Libelle.Contains(rechercheLibelle));
            }

            var produits = await query.ToListAsync();

            var stocksGlobal = new List<StockGlobalViewModel>();

            foreach (var produit in produits)
            {
                // Récupérer tous les mouvements pour ce produit
                var mouvementsQuery = _context.LignesBon
                    .Include(l => l.Bon)
                    .ThenInclude(b => b.DocType)
                    .Where(l => l.IdProduit == produit.IdProduit);

                // Appliquer le filtre de date si spécifié
                if (dateLimite.HasValue)
                {
                    mouvementsQuery = mouvementsQuery.Where(l => l.Bon.Date <= dateLimite.Value);
                }

                var mouvements = await mouvementsQuery.ToListAsync();

                // Calculer les totaux par type de document
                int entreesBE = 0, retourClient = 0, sortiesBS = 0, retourFournisseur = 0;

                foreach (var mouvement in mouvements)
                {
                    var type = mouvement.Bon.DocType.Type?.ToUpper().Trim();

                    switch (type)
                    {
                        case "BE":
                        case "ENTREE":
                            entreesBE += mouvement.Quantite;
                            break;
                        case "RC":
                        case "RETOURCLIENT":
                            retourClient += mouvement.Quantite;
                            break;
                        case "BS":
                        case "SORTIE":
                            sortiesBS += mouvement.Quantite;
                            break;
                        case "RF":
                        case "RETOURFOURNISSEUR":
                            retourFournisseur += mouvement.Quantite;
                            break;
                    }
                }

                // Calculer les totaux
                int sumEntrees = entreesBE + retourClient;
                int sumSorties = sortiesBS + retourFournisseur;
                int stockCalcule = sumEntrees - sumSorties;

                stocksGlobal.Add(new StockGlobalViewModel
                {
                    IdProduit = produit.IdProduit,
                    Libelle = produit.Libelle,
                    Categorie = produit.Categorie?.Nom ?? "Sans catégorie",
                    EntreesBE = entreesBE,
                    RetourClient = retourClient,
                    SortiesBS = sortiesBS,
                    RetourFournisseur = retourFournisseur,
                    SumEntrees = sumEntrees,
                    SumSorties = sumSorties,
                    Stock = stockCalcule,
                    StockPhysique = produit.QuantiteStock,
                    DateCalcul = dateLimite ?? DateTime.Now
                });
            }

            // Récupérer toutes les catégories pour le filtre
            var categories = await _context.Categories.OrderBy(c => c.Nom).ToListAsync();

            return new StockGlobalFilterViewModel
            {
                DateLimite = dateLimite,
                CategorieId = categorieId,
                RechercheLibelle = rechercheLibelle,
                Stocks = stocksGlobal.OrderBy(s => s.Libelle).ToList(),
                Categories = categories,
                TotalProduits = stocksGlobal.Count,
                TotalStock = stocksGlobal.Sum(s => s.Stock),
                TotalEntrees = stocksGlobal.Sum(s => s.SumEntrees),
                TotalSorties = stocksGlobal.Sum(s => s.SumSorties)
            };
        }

        public async Task<List<Categorie>> GetAllCategories()
        {
            return await _context.Categories.OrderBy(c => c.Nom).ToListAsync();
        }
    }
}
