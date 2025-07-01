using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;
using InventoryManagementMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementMVC.Controllers
{
    public class CategorieController : Controller
    {
        private readonly ICategorieService _categorieService;

        public CategorieController(ICategorieService categorieService)
        {
            _categorieService = categorieService;
        }

        // GET: Categorie
        public async Task<IActionResult> Index(string searchString)
        {
            ViewBag.SearchString = searchString;

            var categories = string.IsNullOrEmpty(searchString)
                ? await _categorieService.GetAllCategoriesAsync()
                : await _categorieService.SearchCategoriesAsync(searchString);

            var viewModel = categories.Select(c => new CategorieViewModel
            {
                IdCategorie = c.IdCategorie,
                Nom = c.Nom,
                Description = c.Description,
                NombreProduits = c.Produits.Count()
            });

            return View(viewModel);
        }

        // GET: Categorie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var categorie = await _categorieService.GetCategorieByIdAsync(id.Value);
            if (categorie == null)
                return NotFound();

            var viewModel = new CategorieDetailsViewModel
            {
                IdCategorie = categorie.IdCategorie,
                Nom = categorie.Nom,
                Description = categorie.Description,
                NombreProduits = categorie.Produits.Count(),
                Produits = categorie.Produits.Select(p => new ProduitViewModel
                {
                    IdProduit = p.IdProduit,
                    Libelle = p.Libelle,
                    Description = p.Description,
                    PrixUnitaire = p.PrixUnitaire,
                    QuantiteStock = p.QuantiteStock,
                   
                    NomCategorie = categorie.Nom
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: Categorie/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategorieViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _categorieService.CreateCategorieAsync(model);
                    TempData["Success"] = "Catégorie créée avec succès.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("Nom", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la création de la catégorie: " + ex.Message);
                }
            }

            return View(model);
        }

        // GET: Categorie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var categorie = await _categorieService.GetCategorieByIdAsync(id.Value);
            if (categorie == null)
                return NotFound();

            var model = new EditCategorieViewModel
            {
                Nom = categorie.Nom,
                Description = categorie.Description
            };

            return View(model);
        }

        // POST: Categorie/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateCategorieViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _categorieService.UpdateCategorieAsync(id, model);
                    if (result)
                    {
                        TempData["Success"] = "Catégorie modifiée avec succès.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("Nom", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la modification: " + ex.Message);
                }
            }

            return View(model);
        }

        // GET: Categorie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var categorie = await _categorieService.GetCategorieByIdAsync(id.Value);
            if (categorie == null)
                return NotFound();

            var viewModel = new CategorieViewModel
            {
                IdCategorie = categorie.IdCategorie,
                Nom = categorie.Nom,
                Description = categorie.Description,
                NombreProduits = categorie.Produits.Count()
            };

            return View(viewModel);
        }

        // POST: Categorie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _categorieService.DeleteCategorieAsync(id);
                if (result)
                {
                    TempData["Success"] = "Catégorie supprimée avec succès.";
                }
                else
                {
                    TempData["Error"] = "Catégorie introuvable.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Erreur lors de la suppression: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // AJAX: Vérifier si la catégorie peut être supprimée
        [HttpGet]
        public async Task<IActionResult> CanDelete(int id)
        {
            try
            {
                var hasProducts = await _categorieService.CategorieHasProduitsAsync(id);
                return Json(new { canDelete = !hasProducts });
            }
            catch
            {
                return Json(new { canDelete = false });
            }
        }

        // AJAX: Obtenir le nombre de produits par catégorie
        [HttpGet]
        public async Task<IActionResult> GetProductCount(int id)
        {
            try
            {
                var count = await _categorieService.GetProduitsCountByCategorieAsync(id);
                return Json(new { count = count });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }
    }
}
