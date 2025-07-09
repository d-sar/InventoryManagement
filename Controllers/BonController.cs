using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;
using InventoryManagementMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Controllers
{
    public class BonController : Controller
    {
        private readonly IBonService _bonService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BonController> _logger;

        // Configuration des types de bons
        private readonly Dictionary<string, (string titre, string partenaire, string icon)> _bonConfig = new()
        {
            { "BE", ("Bons d'Entrée", "Fournisseur", "fas fa-arrow-down") },
            { "BS", ("Bons de Sortie", "Client", "fas fa-arrow-up") },
            { "BRF", ("Bons de Retour Fournisseur", "Fournisseur", "fas fa-undo") },
            { "BRC", ("Bons de Retour Client", "Client", "fas fa-undo") }
        };

        public BonController(IBonService bonService, ApplicationDbContext context, ILogger<BonController> logger)
        {
            _bonService = bonService;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string type = "BE", BonFilterViewModel filters = null)
        {
            // Valider le type
            if (!_bonConfig.ContainsKey(type))
            {
                _logger.LogWarning("Type de bon invalide demandé: {Type}", type);
                TempData["ErrorMessage"] = "Type de bon invalide";
                return RedirectToAction("Index", new { type = "BE" });
            }

            try
            {
                var config = _bonConfig[type];
                filters ??= new BonFilterViewModel();
                filters.TypeBon = type;

                // Récupérer les bons selon les filtres
                var bons = await _bonService.GetBonsByTypeAndFiltersAsync(type, filters);

                // Récupérer les partenaires selon le type
                var partenaires = await _context.User
                    .Where(u => u.Type == config.partenaire)
                    .OrderBy(u => u.Nom)
                    .ToListAsync();

                // Calculer les statistiques
                var stats = await _bonService.GetBonStatsAsync(type);

                var viewModel = new BonIndexViewModel
                {
                    Bons = bons,
                    Partenaires = partenaires,
                    Filters = filters,
                    Stats = stats,
                    TypeBon = type,
                    TitrePage = config.titre,
                    TypePartenaire = config.partenaire
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du chargement de l'index des bons - Type: {Type}", type);
                TempData["ErrorMessage"] = "Erreur lors du chargement des données";
                return View(new BonIndexViewModel { TypeBon = type });
            }
        }

        public async Task<IActionResult> Create(string type = "BE")
        {
            if (!_bonConfig.ContainsKey(type))
            {
                _logger.LogWarning("Type de bon invalide pour création: {Type}", type);
                TempData["ErrorMessage"] = "Type de bon invalide";
                return RedirectToAction("Index");
            }

            try
            {
                var viewModel = new BonCreateViewModel { TypeBon = type };
                await PrepareCreateViewData(type);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la préparation de la vue de création - Type: {Type}", type);
                TempData["ErrorMessage"] = "Erreur lors du chargement de la page de création";
                return RedirectToAction("Index", new { type });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BonCreateViewModel model)
        {
            _logger.LogInformation("Données reçues: {@Model}", model);
            _logger.LogInformation("Tentative de création de bon - Type: {TypeBon}, Partenaire: {PartenaireId}, Lignes: {NbLignes}",
                model.TypeBon, model.PartenaireId, model.Lignes?.Count ?? 0);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modèle invalide pour création de bon");
                await PrepareCreateViewData(model.TypeBon);
                return View(model);
            }

            // Validation supplémentaire
            if (model.Lignes == null || !model.Lignes.Any())
            {
                ModelState.AddModelError("", "Le bon doit contenir au moins une ligne");
                await PrepareCreateViewData(model.TypeBon);
                return View(model);
            }

            // Validation des lignes
            for (int i = 0; i < model.Lignes.Count; i++)
            {
                var ligne = model.Lignes[i];
                if (ligne.IdProduit <= 0)
                {
                    ModelState.AddModelError($"Lignes[{i}].IdProduit", "Veuillez sélectionner un produit");
                }
                if (ligne.Quantite <= 0)
                {
                    ModelState.AddModelError($"Lignes[{i}].Quantite", "La quantité doit être positive");
                }
                if (ligne.PrixUnitaire <= 0)
                {
                    ModelState.AddModelError($"Lignes[{i}].PrixUnitaire", "Le prix unitaire doit être positif");
                }
            }

            if (!ModelState.IsValid)
            {
                await PrepareCreateViewData(model.TypeBon);
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

                _logger.LogInformation("Création du bon en cours...");
                var bon = await _bonService.CreateBonAsync(model.TypeBon, model.PartenaireId, lignes);

                _logger.LogInformation("Mise à jour du stock en cours...");
                await _bonService.UpdateStockFromBonAsync(bon.IdBon, model.TypeBon);

                _logger.LogInformation("Bon créé avec succès - ID: {BonId}", bon.IdBon);
                TempData["SuccessMessage"] = $"{_bonConfig[model.TypeBon].titre.TrimEnd('s')} créé avec succès";
                return RedirectToAction(nameof(Details), new { id = bon.IdBon, type = model.TypeBon });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erreur de validation lors de la création du bon");
                ModelState.AddModelError("", ex.Message);
                await PrepareCreateViewData(model.TypeBon);
                return View(model);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erreur d'argument lors de la création du bon");
                ModelState.AddModelError("", ex.Message);
                await PrepareCreateViewData(model.TypeBon);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la création du bon");
                ModelState.AddModelError("", "Une erreur inattendue s'est produite. Veuillez réessayer.");
                await PrepareCreateViewData(model.TypeBon);
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id, string type = "BE")
        {
            try
            {
                var bon = await _bonService.GetBonByIdAsync(id);
                if (bon == null)
                {
                    _logger.LogWarning("Bon introuvable pour affichage détails - ID: {BonId}", id);
                    TempData["ErrorMessage"] = "Bon introuvable";
                    return RedirectToAction(nameof(Index), new { type });
                }

                ViewBag.TypeBon = type;
                ViewBag.TitrePage = _bonConfig.GetValueOrDefault(type).titre ?? "Détails du Bon";
                return View(bon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'affichage des détails du bon - ID: {BonId}", id);
                TempData["ErrorMessage"] = "Erreur lors du chargement des détails";
                return RedirectToAction(nameof(Index), new { type });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string type = "BE")
        {
            _logger.LogInformation("Tentative de suppression du bon - ID: {BonId}", id);

            try
            {
                var success = await _bonService.DeleteBonAsync(id);
                if (success)
                {
                    _logger.LogInformation("Bon supprimé avec succès - ID: {BonId}", id);
                    TempData["SuccessMessage"] = $"{_bonConfig[type].titre.TrimEnd('s')} supprimé avec succès";
                }
                else
                {
                    _logger.LogWarning("Tentative de suppression d'un bon inexistant - ID: {BonId}", id);
                    TempData["ErrorMessage"] = "Bon introuvable";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du bon - ID: {BonId}", id);
                TempData["ErrorMessage"] = $"Erreur lors de la suppression : {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { type });
        }

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

        public async Task<IActionResult> ExportToPdf(int id, string type = "BE")
        {
            var bon = await _bonService.GetBonByIdAsync(id);
            if (bon == null)
                return NotFound();

            ViewBag.TypeBon = type;
            ViewBag.TitrePage = _bonConfig.GetValueOrDefault(type).titre ?? "Bon";
            return View("PrintBon", bon);
        }

        private async Task PrepareCreateViewData(string type)
        {
            var config = _bonConfig[type];

            ViewBag.Partenaires = await _context.User
                .Where(u => u.Type == config.partenaire)
                .OrderBy(u => u.Nom)
                .ToListAsync();

            ViewBag.Produits = await _context.Produits
                .Include(p => p.Categorie)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Libelle)
                .ToListAsync();

            ViewBag.TypeBon = type;
            ViewBag.TitrePage = $"Nouveau {config.titre.TrimEnd('s')}";
            ViewBag.TypePartenaire = config.partenaire;
        }
    }
}
