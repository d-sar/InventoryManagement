using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;
using InventoryManagementMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementMVC.Controllers
{
    public class FournisseurController : Controller
    {
        private readonly IFournisseurService _fournisseurService;

        public FournisseurController(IFournisseurService fournisseurService)
        {
            _fournisseurService = fournisseurService;
        }

        public async Task<IActionResult> Index()
        {
            var fournisseurs = await _fournisseurService.GetAllFournisseursAsync();
            var model = fournisseurs.Select(f => new FournisseurViewModel
            {
                Id = f.Id,
                Nom = f.Nom,
                Adresse = f.Adresse,
                Telephone = f.Telephone,
                Email = f.Email,
                Type = f.Type,
                NombreBons = f.Bons.Count,
                TotalFournitures = f.Bons.Sum(b => b.LignesBon.Sum(l => l.Quantite * l.PrixUnitaire))
            });
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var fournisseur = await _fournisseurService.GetFournisseurByIdAsync(id);
            if (fournisseur == null) return NotFound();

            var model = new FournisseurViewModel
            {
                Id = fournisseur.Id,
                Nom = fournisseur.Nom,
                Adresse = fournisseur.Adresse,
                Telephone = fournisseur.Telephone,
                Email = fournisseur.Email,
                Type = fournisseur.Type,
                NombreBons = fournisseur.Bons.Count,
                //TotalFournitures = fournisseur.Bons.Sum(b => b.LignesBon.Sum(l => l.Quantite * l.PrixUnitaire))
            };
            return View(model);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FournisseurViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var fournisseur = new Fournisseur
            {
                Nom = model.Nom,
                Adresse = model.Adresse,
                Telephone = model.Telephone,
                Email = model.Email,
                Type = "Fournisseur"
            };
            await _fournisseurService.AddFournisseurAsync(fournisseur);
            TempData["Success"] = "Fournisseur créé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var fournisseur = await _fournisseurService.GetFournisseurByIdAsync(id);
            if (fournisseur == null) return NotFound();

            var model = new FournisseurViewModel
            {
                Id = fournisseur.Id,
                Nom = fournisseur.Nom,
                Adresse = fournisseur.Adresse,
                Telephone = fournisseur.Telephone,
                Email = fournisseur.Email,
                Type = fournisseur.Type
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FournisseurViewModel model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var fournisseur = await _fournisseurService.GetFournisseurByIdAsync(id);
            if (fournisseur == null) return NotFound();

            fournisseur.Nom = model.Nom;
            fournisseur.Adresse = model.Adresse;
            fournisseur.Telephone = model.Telephone;
            fournisseur.Email = model.Email;

            await _fournisseurService.UpdateFournisseurAsync(fournisseur);
            TempData["Success"] = "Fournisseur modifié avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var fournisseur = await _fournisseurService.GetFournisseurByIdAsync(id);
            if (fournisseur == null) return NotFound();

            var model = new FournisseurViewModel
            {
                Id = fournisseur.Id,
                Nom = fournisseur.Nom,
                Adresse = fournisseur.Adresse,
                Telephone = fournisseur.Telephone,
                Email = fournisseur.Email,
                Type = fournisseur.Type
            };
            return View(model);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _fournisseurService.DeleteFournisseurAsync(id);
            TempData["Success"] = "Fournisseur supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }
    }
}

