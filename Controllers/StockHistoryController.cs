using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Controllers
{
    public class StockHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? idProduit, DateTime? dateDebut, DateTime? dateFin)
        {
            var produits = await _context.Produits.ToListAsync();
            StockHistoryViewModel historique = null;

            if (idProduit.HasValue && dateDebut.HasValue && dateFin.HasValue)
            {
                var produit = await _context.Produits.FindAsync(idProduit.Value);
                if (produit == null) return NotFound();

                var mouvementsAvant = await _context.LigneBons
                    .Include(l => l.Bon)
                    .ThenInclude(b => b.DocType)
                    .Where(l => l.IdProduit == idProduit.Value && l.Bon.Date < dateDebut)
                    .ToListAsync();

                int stockInitial = 0;
                foreach (var mouvement in mouvementsAvant)
                {
                    var type = mouvement.Bon.DocType.Type?.ToUpper().Trim();
                    int quantite = mouvement.Quantite;

                    if (type == "ENTREE" || type == "RETOURCLIENT")
                        stockInitial += quantite;
                    else if (type == "SORTIE" || type == "RETOURFOURNISSEUR")
                        stockInitial -= quantite;
                }

                var mouvements = await _context.LigneBons
                    .Include(l => l.Bon)
                    .ThenInclude(b => b.DocType)
                    .Where(l => l.IdProduit == idProduit.Value
                                && l.Bon.Date >= dateDebut
                                && l.Bon.Date <= dateFin)
                    .OrderBy(l => l.Bon.Date)
                    .ToListAsync();

                historique = new StockHistoryViewModel
                {
                    Produit = produit,
                    DateDebut = dateDebut.Value,
                    DateFin = dateFin.Value,
                    StockInitial = stockInitial,
                    Mouvements = mouvements
                };
            }

            ViewBag.Produits = produits;
            return View(historique);
        }



    }
}
