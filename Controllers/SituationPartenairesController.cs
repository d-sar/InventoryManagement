using InventoryManagementMVC.Models.ViewModels.SituationPartenaires;
using InventoryManagementMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementMVC.Controllers
{
    public class SituationPartenairesController : Controller
    {
        private readonly ISituationPartenairesService _situationService;

        public SituationPartenairesController(ISituationPartenairesService situationService)
        {
            _situationService = situationService;
        }

        public async Task<IActionResult> Index()
        {
            var allPartenaires = await _situationService.GetAllPartenairesAsync();

            var viewModel = new SituationPartenairesViewModel
            {
                Partenaires = allPartenaires,
                Filter = new SituationFilterViewModel()
            };

            // Si la liste n'est pas vide, sélectionner automatiquement le premier partenaire
            if (allPartenaires.Any())
            {
                var premier = allPartenaires.First();
                viewModel.Filter.TypePartenaire = premier.Type;
                viewModel.Filter.PartenaireId = premier.Id;
                viewModel.Filter.DateDebut = DateTime.Now.AddMonths(-1);
                viewModel.Filter.DateFin = DateTime.Now;

                // Charger la situation du premier partenaire
                viewModel.Result = await _situationService.GetSituationPartenaireAsync(
                    premier.Id,
                    viewModel.Filter.DateDebut,
                    viewModel.Filter.DateFin
                );
            }

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Index(SituationFilterViewModel filter)
        {
            var viewModel = new SituationPartenairesViewModel
            {
                Filter = filter
            };

            // Charger les partenaires selon le type sélectionné
            if (!string.IsNullOrEmpty(filter.TypePartenaire))
            {
                viewModel.Partenaires = await _situationService.GetPartenairesByTypeAsync(filter.TypePartenaire);
            }
            else
            {
                viewModel.Partenaires = await _situationService.GetAllPartenairesAsync();
            }

            // Si un partenaire est sélectionné, calculer sa situation
            if (filter.PartenaireId.HasValue)
            {
                viewModel.Result = await _situationService.GetSituationPartenaireAsync(
                    filter.PartenaireId.Value,
                    filter.DateDebut,
                    filter.DateFin
                );

                if (viewModel.Result == null)
                {
                    ModelState.AddModelError("", "Partenaire introuvable.");
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetPartenairesByType(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                var allPartenaires = await _situationService.GetAllPartenairesAsync();
                return Json(allPartenaires);
            }

            var partenaires = await _situationService.GetPartenairesByTypeAsync(type);
            return Json(partenaires);
        }

        [HttpGet]
        public async Task<IActionResult> GetSituation(int partenaireId, DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            var situation = await _situationService.GetSituationPartenaireAsync(partenaireId, dateDebut, dateFin);

            if (situation == null)
            {
                return NotFound();
            }

            return PartialView("_SituationResult", situation);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? partenaireId, string type = "Client")
        {
            var partenaires = await _situationService.GetPartenairesByTypeAsync(type);

            var viewModel = new SituationPartenairesViewModel
            {
                Partenaires = partenaires,
                Filter = new SituationFilterViewModel
                {
                    TypePartenaire = type,
                    PartenaireId = partenaireId,
                    DateDebut = DateTime.Now.AddMonths(-1),
                    DateFin = DateTime.Now
                }
            };

            if (partenaireId.HasValue)
            {
                viewModel.Result = await _situationService.GetSituationPartenaireAsync(
                    partenaireId.Value,
                    viewModel.Filter.DateDebut,
                    viewModel.Filter.DateFin
                );
            }

            return View(viewModel);
        }


    }
}
