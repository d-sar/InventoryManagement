using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;
using InventoryManagementMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementMVC.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(IClientService clientService, ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        // GET: Client
        public async Task<IActionResult> Index(string searchString)
        {
            try
            {
                IEnumerable<Client> clients;

                if (!string.IsNullOrEmpty(searchString))
                {
                    clients = await _clientService.SearchClientsAsync(searchString);
                    ViewData["CurrentFilter"] = searchString;
                }
                else
                {
                    clients = await _clientService.GetAllClientsAsync();
                }

                var model = clients.Select(c => new ClientViewModel
                {
                    Id = c.Id,
                    Nom = c.Nom,
                    Adresse = c.Adresse,
                    Telephone = c.Telephone,
                    Email = c.Email,
                    Type = c.Type,
                    EstActif = c.EstActif,
                    DateInscription = c.DateInscription,
                    NombreBons = c.Bons?.Count ?? 0,
                    TotalAchats =
    (c.Bons?.Where(b => b.DocType != null && b.DocType.Type == "Sortie")
            .Sum(b => b.LignesBon?.Sum(l => l.Quantite * l.PrixUnitaire) ?? 0) ?? 0)
    -
    (c.Bons?.Where(b => b.DocType != null && b.DocType.Type == "RetourClient")
            .Sum(b => b.LignesBon?.Sum(l => l.Quantite * l.PrixUnitaire) ?? 0) ?? 0),
                    //DernierAchat = c.Bons?.OrderByDescending(b => b.DateBon).FirstOrDefault()?.DateBon
                });

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des clients");
                TempData["Error"] = "Une erreur s'est produite lors de la récupération des clients.";
                return View(new List<ClientViewModel>());
            }
        }

        // GET: Client/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    TempData["Error"] = "Client non trouvé.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new ClientViewModel
                {
                    Id = client.Id,
                    Nom = client.Nom,
                    Adresse = client.Adresse,
                    Telephone = client.Telephone,
                    Email = client.Email,
                    Type = client.Type,
                    EstActif = client.EstActif,
                    DateInscription = client.DateInscription,
                    NombreBons = client.Bons?.Count ?? 0,
                    TotalAchats = client.Bons?.Sum(b => b.LignesBon?.Sum(l => l.Quantite * l.PrixUnitaire) ?? 0) ?? 0,
                    //DernierAchat = client.Bons?.OrderByDescending(b => b.DateBon).FirstOrDefault()?.DateBon
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du client {ClientId}", id);
                TempData["Error"] = "Une erreur s'est produite lors de la récupération du client.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Client/Create
        public IActionResult Create()
        {
            return View(new ClientViewModel { EstActif = true });
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = new Client
                {
                    Nom = model.Nom,
                    Adresse = model.Adresse,
                    Telephone = model.Telephone,
                    Email = model.Email,
                    EstActif = model.EstActif
                };

                await _clientService.AddClientAsync(client);
                TempData["Success"] = "Client créé avec succès.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du client");
                TempData["Error"] = "Une erreur s'est produite lors de la création du client.";
                return View(model);
            }
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    TempData["Error"] = "Client non trouvé.";
                    return RedirectToAction(nameof(Index));
                }

                var model = new ClientViewModel
                {
                    Id = client.Id,
                    Nom = client.Nom,
                    Adresse = client.Adresse,
                    Telephone = client.Telephone,
                    Email = client.Email,
                    Type = client.Type,
                    EstActif = client.EstActif,
                    DateInscription = client.DateInscription
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du client {ClientId} pour modification", id);
                TempData["Error"] = "Une erreur s'est produite lors de la récupération du client.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Client/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientViewModel model)
        {
            if (id != model.Id)
            {
                TempData["Error"] = "Identifiant du client invalide.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    TempData["Error"] = "Client non trouvé.";
                    return RedirectToAction(nameof(Index));
                }

                client.Nom = model.Nom;
                client.Adresse = model.Adresse;
                client.Telephone = model.Telephone;
                client.Email = model.Email;
                client.EstActif = model.EstActif;

                await _clientService.UpdateClientAsync(client);
                TempData["Success"] = "Client modifié avec succès.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la modification du client {ClientId}", id);
                TempData["Error"] = "Une erreur s'est produite lors de la modification du client.";
                return View(model);
            }
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    TempData["Error"] = "Client non trouvé.";
                    return RedirectToAction(nameof(Index));
                }

                var model = new ClientViewModel
                {
                    Id = client.Id,
                    Nom = client.Nom,
                    Adresse = client.Adresse,
                    Telephone = client.Telephone,
                    Email = client.Email,
                    Type = client.Type,
                    EstActif = client.EstActif,
                    DateInscription = client.DateInscription,
                    NombreBons = client.Bons?.Count ?? 0,
                    TotalAchats = client.Bons?.Sum(b => b.LignesBon?.Sum(l => l.Quantite * l.PrixUnitaire) ?? 0) ?? 0
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du client {ClientId} pour suppression", id);
                TempData["Error"] = "Une erreur s'est produite lors de la récupération du client.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _clientService.DeleteClientAsync(id);
                if (success)
                {
                    TempData["Success"] = "Client supprimé avec succès.";
                }
                else
                {
                    TempData["Error"] = "Client non trouvé.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du client {ClientId}", id);
                TempData["Error"] = "Une erreur s'est produite lors de la suppression du client.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Action pour activer/désactiver un client
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    return Json(new { success = false, message = "Client non trouvé." });
                }

                client.EstActif = !client.EstActif;
                await _clientService.UpdateClientAsync(client);

                return Json(new
                {
                    success = true,
                    message = client.EstActif ? "Client activé avec succès." : "Client désactivé avec succès.",
                    newStatus = client.EstActif
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du changement de statut du client {ClientId}", id);
                return Json(new { success = false, message = "Une erreur s'est produite." });
            }
        }
    }
}

