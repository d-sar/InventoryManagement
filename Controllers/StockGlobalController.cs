using InventoryManagementMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementMVC.Controllers
{
    public class StockGlobalController : Controller
    {
        private readonly StockGlobalService _stockGlobalService;

        public StockGlobalController(StockGlobalService stockGlobalService)
        {
            _stockGlobalService = stockGlobalService;
        }

        public async Task<IActionResult> Index(
            DateTime? dateLimite,
            int? categorieId,
            string rechercheLibelle)
        {
            var model = await _stockGlobalService.GetStockGlobal(
                dateLimite,
                categorieId,
                rechercheLibelle);

            return View(model);
        }

        
    }
}
