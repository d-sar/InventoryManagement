using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
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
            var defaultDateDebut = DateTime.Today.AddDays(-30);
            var defaultDateFin = DateTime.Today;
            var defaultProduit = await _context.Produits.FirstOrDefaultAsync();

            if (defaultProduit == null)
            {
                // Aucun produit trouvé : afficher un message ou une vue vide
                ViewBag.Produits = new List<Produit>();
                return View(null);
            }

            // Si l'un des paramètres est manquant, rediriger avec des valeurs par défaut
            if (!idProduit.HasValue || !dateDebut.HasValue || !dateFin.HasValue)
            {
                return RedirectToAction("Index", new
                {
                    idProduit = idProduit ?? defaultProduit.IdProduit,
                    dateDebut = (dateDebut ?? defaultDateDebut).ToString("yyyy-MM-dd"),
                    dateFin = (dateFin ?? defaultDateFin).ToString("yyyy-MM-dd")
                });
            }

            var produits = await _context.Produits.ToListAsync();
            var produit = await _context.Produits.FindAsync(idProduit.Value);
            if (produit == null) return NotFound();

            // Calcul du stock initial
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

            // Mouvements pendant la période
            var mouvements = await _context.LigneBons
                .Include(l => l.Bon)
                .ThenInclude(b => b.DocType)
                .Where(l => l.IdProduit == idProduit.Value
                            && l.Bon.Date >= dateDebut
                            && l.Bon.Date <= dateFin)
                .OrderBy(l => l.Bon.Date)
                .ToListAsync();

            var historique = new StockHistoryViewModel
            {
                Produit = produit,
                DateDebut = dateDebut.Value,
                DateFin = dateFin.Value,
                StockInitial = stockInitial,
                Mouvements = mouvements
            };

            ViewBag.Produits = produits;
            return View(historique);
        }


    }
}
