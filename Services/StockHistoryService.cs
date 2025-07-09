using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Services
{
    public class StockHistoryService
    {
        private readonly ApplicationDbContext _context;

        public StockHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MouvementStockViewModel>> GetHistoriqueMouvements(int produitId, DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            var produit = await _context.Produits
                .Include(p => p.Categorie)
                .FirstOrDefaultAsync(p => p.IdProduit == produitId);

            if (produit == null)
                return new List<MouvementStockViewModel>();

            // Récupérer TOUS les mouvements pour ce produit (pour calculer le stock initial)
            var tousLesMouvements = await _context.LignesBon
                .Include(l => l.Bon)
                .ThenInclude(b => b.DocType)
                .Include(l => l.Produit)
                .Where(l => l.IdProduit == produitId)
                .OrderBy(l => l.Bon.Date)
                .ThenBy(l => l.Bon.IdBon)
                .ToListAsync();

            // Calculer le stock initial (mouvements avant la date de début)
            int stockInitial = 0;
            if (dateDebut.HasValue)
            {
                var mouvementsAvantFiltre = tousLesMouvements
                    .Where(m => m.Bon.Date < dateDebut.Value)
                    .ToList();

                foreach (var mouvement in mouvementsAvantFiltre)
                {
                    if (mouvement.Bon.DocType.Type == "BE" || mouvement.Bon.DocType.Type == "RF")
                    {
                        stockInitial += mouvement.Quantite;
                    }
                    else if (mouvement.Bon.DocType.Type == "BS" || mouvement.Bon.DocType.Type == "RC")
                    {
                        stockInitial -= mouvement.Quantite;
                    }
                }
            }

            // Récupérer les mouvements dans la période filtrée
            var mouvementsFiltres = tousLesMouvements
                .Where(l => dateDebut == null || l.Bon.Date >= dateDebut)
                .Where(l => dateFin == null || l.Bon.Date <= dateFin)
                .ToList();

            var historiqueList = new List<MouvementStockViewModel>();
            int stockCourant = stockInitial;

            // Grouper par bon pour afficher une ligne par bon
            var mouvementsGroupes = mouvementsFiltres
                .GroupBy(m => new { m.Bon.IdBon, m.Bon.Numero, m.Bon.Date, m.Bon.DocType.Type })
                .OrderBy(g => g.Key.Date)
                .ThenBy(g => g.Key.IdBon);

            foreach (var groupe in mouvementsGroupes)
            {
                var quantiteTotale = groupe.Sum(m => m.Quantite);
                int entreesBE = 0, entreesRF = 0, sortiesBS = 0, sortiesRC = 0;

                // Calculer les entrées et sorties selon le type
                switch (groupe.Key.Type)
                {
                    case "BE":
                        entreesBE = quantiteTotale;
                        stockCourant += quantiteTotale;
                        break;
                    case "RF":
                        entreesRF = quantiteTotale;
                        stockCourant += quantiteTotale;
                        break;
                    case "BS":
                        sortiesBS = quantiteTotale;
                        stockCourant -= quantiteTotale;
                        break;
                    case "RC":
                        sortiesRC = quantiteTotale;
                        stockCourant -= quantiteTotale;
                        break;
                }

                historiqueList.Add(new MouvementStockViewModel
                {
                    Article = produit.Libelle,
                    StockInitial = stockInitial,
                    NumeroDocument = groupe.Key.Numero,
                    Date = groupe.Key.Date,
                    EntreesBE = entreesBE > 0 ? entreesBE : (int?)null,
                    EntreesRF = entreesRF > 0 ? entreesRF : (int?)null,
                    SortiesBS = sortiesBS > 0 ? sortiesBS : (int?)null,
                    SortiesRC = sortiesRC > 0 ? sortiesRC : (int?)null,
                    Stock = stockCourant,
                    TypeDocument = groupe.Key.Type
                });
            }

            // Calculer le stock final (stock actuel du produit)
            int stockFinal = produit.QuantiteStock;

            // Mettre à jour le stock final pour tous les mouvements
            foreach (var mouvement in historiqueList)
            {
                mouvement.StockFinal = stockFinal;
            }

            return historiqueList;
        }

        public async Task<List<Produit>> GetAllProduits()
        {
            return await _context.Produits
                .Include(p => p.Categorie)
                .OrderBy(p => p.Libelle)
                .ToListAsync();
        }
        public async Task<List<DebugMouvementViewModel>> GetDebugMouvements(int produitId)
        {
            var mouvements = await _context.LignesBon
                .Include(l => l.Bon)
                .ThenInclude(b => b.DocType)
                .Include(l => l.Produit)
                .Where(l => l.IdProduit == produitId)
                .OrderBy(l => l.Bon.Date)
                .Select(l => new DebugMouvementViewModel
                {
                    NumeroDocument = l.Bon.Numero,
                    Date = l.Bon.Date,
                    TypeDocument = l.Bon.DocType.Type,
                    Quantite = l.Quantite,
                    ProduitLibelle = l.Produit.Libelle
                })
                .ToListAsync();

            return mouvements;
        }

    }
}
