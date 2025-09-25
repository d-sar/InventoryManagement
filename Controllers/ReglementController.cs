using InventoryManagementMVC.Models.ViewModels.gestiondesReglements;
using InventoryManagementMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Controllers
{
    public class ReglementController : Controller
    {
        private readonly IReglementService _reglementService;
        

        public ReglementController(IReglementService reglementService)
        {
            _reglementService = reglementService;
        }

        // GET: Reglement
        public async Task<IActionResult> Index(ReglementFilters filters)
        {
            var viewModel = await _reglementService.GetReglementIndexViewModelAsync(filters);
            return View(viewModel);
        }

        // GET: Reglement/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _reglementService.GetDetailsViewModelAsync(id);

            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Règlement introuvable.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Reglement/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = await _reglementService.GetCreateViewModelAsync();
            return View(viewModel);
        }

        // POST: Reglement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReglementCreateEditViewModel viewModel)
        {
            Console.WriteLine("Form submitted"); // Ou via logger

            if (ModelState.IsValid)
            {
                Console.WriteLine("Model is not valid");
                var success = await _reglementService.CreateReglementAsync(viewModel.Reglement);

                if (success)
                {
                    TempData["SuccessMessage"] = "Règlement créé avec succès.";
                   
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    TempData["ErrorMessage"] = "Erreur lors de la création du règlement.";
                }
            }
            foreach (var modelStateKey in ModelState.Keys)
            {
                var value = ModelState[modelStateKey];
                foreach (var error in value.Errors)
                {
                    Console.WriteLine($"Erreur pour {modelStateKey} : {error.ErrorMessage}");
                }
            }
            Console.WriteLine("Model is valid");

            // Recharger les données nécessaires en cas d'erreur
            viewModel = await _reglementService.GetCreateViewModelAsync();
            return View(viewModel);
        }

        // GET: Reglement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = await _reglementService.GetEditViewModelAsync(id);

            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Règlement introuvable.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // POST: Reglement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReglementCreateEditViewModel viewModel)
        {
            if (id != viewModel.Reglement.IdReglement)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var success = await _reglementService.UpdateReglementAsync(viewModel.Reglement);

                if (success)
                {
                    TempData["SuccessMessage"] = "Règlement modifié avec succès.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Erreur lors de la modification du règlement.";
                }
            }

            // Recharger les données nécessaires en cas d'erreur
            viewModel = await _reglementService.GetEditViewModelAsync(id);
            return View(viewModel);
        }

        // POST: Reglement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _reglementService.DeleteReglementAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Règlement supprimé avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = "Erreur lors de la suppression du règlement.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Reglement/ByPartenaire/5
        public async Task<IActionResult> ByPartenaire(int partenaireId)
        {
            var reglements = await _reglementService.GetReglementsByPartenaireAsync(partenaireId);
            return PartialView("ByPartenaire", reglements);
        }

       


        // AJAX: Get stats for dashboard
        [HttpGet]
        public async Task<IActionResult> GetStats(ReglementFilters filters)
        {
            var stats = await _reglementService.GetStatsAsync(filters);
            return Json(stats);
        }
    }
}