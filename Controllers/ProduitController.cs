
using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.ViewModels;
using InventoryManagementMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Controllers
{
    public class ProduitController : Controller
    {
        private readonly IProduitService _produitService;
        private readonly ApplicationDbContext _context;
      
        public ProduitController(IProduitService produitService, ApplicationDbContext context)
        {
            _produitService = produitService;
            _context = context;
        }

        // GET: Produit
        public async Task<IActionResult> Index(string searchString, int? categorieId)
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "IdCategorie", "Nom");
            ViewBag.SearchString = searchString;
            ViewBag.CategorieId = categorieId;

            var produits = await _produitService.GetAllProduitsAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                produits = await _produitService.SearchProduitsAsync(searchString);
            }
            else if (categorieId.HasValue)
            {
                produits = await _produitService.GetProduitsByCategorieAsync(categorieId.Value);
            }

            var viewModel = produits.Select(p => new ProduitViewModel
            {
                IdProduit = p.IdProduit,
                Libelle = p.Libelle,
                Description = p.Description,
                PrixUnitaire = p.PrixUnitaire,
                QuantiteStock = p.QuantiteStock,
                
                NomCategorie = p.Categorie.Nom
            });

            return View(viewModel);
        }

        // GET: Produit/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var produit = await _produitService.GetProduitByIdAsync(id.Value);
            if (produit == null)
                return NotFound();

            var viewModel = new ProduitViewModel
            {
                IdProduit = produit.IdProduit,
                Libelle = produit.Libelle,
                Description = produit.Description,
                PrixUnitaire = produit.PrixUnitaire,
                QuantiteStock = produit.QuantiteStock,
                
                NomCategorie = produit.Categorie.Nom
            };

            return View(viewModel);
        }

        // GET: Produit/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "IdCategorie", "Nom");
            return View();
        }

        // POST: Produit/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProduitViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _produitService.CreateProduitAsync(model);
                    TempData["Success"] = "Produit créé avec succès.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la création du produit: " + ex.Message);
                }
            }

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "IdCategorie", "Nom", model.IdCategorie);
            return View(model);
        }

        // GET: Produit/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var produit = await _produitService.GetProduitByIdAsync(id.Value);
            if (produit == null)
                return NotFound();

            var model = new EditProduitViewModel
            {
                IdCategorie = produit.IdCategorie,
                Libelle = produit.Libelle,
                Description = produit.Description,
                PrixUnitaire = produit.PrixUnitaire,
                QuantiteStock = produit.QuantiteStock
            };

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "IdCategorie", "Nom", produit.IdCategorie);
            return View(model);
        }

        // POST: Produit/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateProduitViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _produitService.UpdateProduitAsync(id, model);
                    if (result)
                    {
                        TempData["Success"] = "Produit modifié avec succès.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la modification: " + ex.Message);
                }
            }

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "IdCategorie", "Nom", model.IdCategorie);
            return View(model);
        }

        // GET: Produit/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var produit = await _produitService.GetProduitByIdAsync(id.Value);
            if (produit == null)
                return NotFound();

            var viewModel = new ProduitViewModel
            {
                IdProduit = produit.IdProduit,
                Libelle = produit.Libelle,
                Description = produit.Description,
                PrixUnitaire = produit.PrixUnitaire,
                QuantiteStock = produit.QuantiteStock,
                NomCategorie = produit.Categorie.Nom
            };

            return View(viewModel);
        }

        // POST: Produit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _produitService.DeleteProduitAsync(id);
                if (result)
                {
                    TempData["Success"] = "Produit supprimé avec succès.";
                }
                else
                {
                    TempData["Error"] = "Erreur lors de la suppression.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Erreur lors de la suppression: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // AJAX: Mise à jour du stock
        [HttpPost]
        public async Task<IActionResult> UpdateStock(int id, int nouvelleQuantite)
        {
            try
            {
                var result = await _produitService.UpdateStockAsync(id, nouvelleQuantite);
                return Json(new { success = result });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}