using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;
using InventoryManagementMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Controllers
{
    public class BonEntreeController : Controller
    {
        private readonly IBonEntreeService _bonEntreeService;
        private readonly ApplicationDbContext _context;

        public BonEntreeController(IBonEntreeService bonEntreeService, ApplicationDbContext context)
        {
            _bonEntreeService = bonEntreeService;
            _context = context;
        }

        public async Task<IActionResult> Index(BonEntreeFilterViewModel filter)
        {
            var query = _context.Bons
                .Include(b => b.Partenaire)
                .Include(b => b.DocType)
                .Include(b => b.LignesBon)
                .Where(b => b.DocType.Type == "Entree");

            // Appliquer les filtres
            if (filter.FournisseurId.HasValue)
                query = query.Where(b => b.IdUser == filter.FournisseurId.Value);

            if (filter.DateDebut.HasValue)
                query = query.Where(b => b.Date >= filter.DateDebut.Value);

            if (filter.DateFin.HasValue)
                query = query.Where(b => b.Date <= filter.DateFin.Value);

            if (filter.Numero.HasValue)
                query = query.Where(b => b.Numero == filter.Numero.Value);

            var bons = await query.OrderByDescending(b => b.Date).ToListAsync();

            ViewBag.Fournisseurs = await _context.Fournisseurs.ToListAsync();
            ViewBag.Filter = filter;

            return View(bons);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new BonCreateViewModel();
            await PrepareCreateViewData();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BonCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareCreateViewData();
                return View(model);
            }

            try
            {
                var lignes = model.Lignes.Select(l => new LigneBon
                {
                    IdProduit = l.IdProduit,
                    Quantite = l.Quantite,
                    PrixUnitaire = l.PrixUnitaire
                }).ToList();

                var bon = await _bonEntreeService.CreateBonEntreeAsync(model.FournisseurId, lignes);
                await _bonEntreeService.UpdateStockFromBonEntreeAsync(bon.IdBon);

                TempData["SuccessMessage"] = "Bon d'entrée créé avec succès";
                return RedirectToAction(nameof(Details), new { id = bon.IdBon });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erreur lors de la création du bon: {ex.Message}");
                await PrepareCreateViewData();
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var bon = await _bonEntreeService.GetBonEntreeByIdAsync(id);
            if (bon == null)
            {
                TempData["ErrorMessage"] = "Bon d'entrée introuvable";
                return RedirectToAction(nameof(Index));
            }

            return View(bon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _bonEntreeService.DeleteBonEntreeAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Bon d'entrée supprimé avec succès";
                }
                else
                {
                    TempData["ErrorMessage"] = "Bon d'entrée introuvable";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erreur lors de la suppression: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Action AJAX pour obtenir les informations d'un produit
        [HttpGet]
        public async Task<IActionResult> GetProduitInfo(int id)
        {
            var produit = await _context.Produits
                .Include(p => p.Categorie)
                .FirstOrDefaultAsync(p => p.IdProduit == id);

            if (produit == null)
                return NotFound();

            return Json(new
            {
                id = produit.IdProduit,
                libelle = produit.Libelle,
                prixUnitaire = produit.PrixUnitaire,
                quantiteStock = produit.QuantiteStock,
                categorie = produit.Categorie.Nom
            });
        }

        // Méthode d'aide pour préparer les données de la vue Create
        private async Task PrepareCreateViewData()
        {
            ViewBag.Fournisseurs = await _context.Fournisseurs
                .Where(f => f.Type == "Fournisseur")
                .OrderBy(f => f.Nom)
                .ToListAsync();

            ViewBag.Produits = await _context.Produits
                .Include(p => p.Categorie)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Libelle)
                .ToListAsync();
        }

        // Action pour exporter en PDF (optionnel)
        public async Task<IActionResult> ExportToPdf(int id)
        {
            var bon = await _bonEntreeService.GetBonEntreeByIdAsync(id);
            if (bon == null)
                return NotFound();

            // Ici vous pouvez implémenter la génération de PDF
            // avec une bibliothèque comme iTextSharp ou DinkToPdf

            return View("PrintBon", bon);
        }

        // GET: BonEntree/Print/5
        public async Task<IActionResult> Print(int id)
        {
            var bon = await _bonEntreeService.GetBonEntreeByIdAsync(id);

            if (bon == null)
            {
                TempData["ErrorMessage"] = "Bon d'entrée introuvable pour impression.";
                return RedirectToAction(nameof(Index));
            }

            return View("PrintBon", bon); // Assurez-vous d’avoir une vue Razor nommée PrintBon.cshtml
        }

    }
}
