using InventoryManagementMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementMVC.Controllers
{
    public class StockValoriseController : Controller
    {
       
            private readonly StockValoriseService _stockValoriseService;

            public StockValoriseController(StockValoriseService stockValoriseService)
            {
                _stockValoriseService = stockValoriseService;
            }

            public async Task<IActionResult> Index(
                DateTime? dateLimite,
                int? categorieId,
                string rechercheLibelle)
            {
                var model = await _stockValoriseService.GetStockValorise(
                    dateLimite,
                    categorieId,
                    rechercheLibelle);

                return View(model);
            }

            [HttpGet]
            public async Task<IActionResult> Details(int id, DateTime? dateLimite)
            {
                try
                {
                    var model = await _stockValoriseService.GetDetailsProduit(id, dateLimite);
                    return View(model);
                }
                catch (ArgumentException)
                {
                    return NotFound();
                }
            }
    }
}
