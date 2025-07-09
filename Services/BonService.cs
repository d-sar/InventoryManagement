using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementMVC.Services
{
    public class BonService : IBonService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BonService> _logger;
        private readonly Dictionary<string, string> _typeMapping = new()
        {
            { "BE", "Entree" },
            { "BS", "Sortie" },
            { "BRF", "RetourFournisseur" },
            { "BRC", "RetourClient" }
        };

        public BonService(ApplicationDbContext context, ILogger<BonService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Bon> CreateBonAsync(string typeBon, int partenaireId, List<LigneBon> lignes)
        {
            _logger.LogInformation("Début création bon - Type: {TypeBon}, Partenaire: {PartenaireId}, Lignes: {NbLignes}",
                typeBon, partenaireId, lignes.Count);

            if (lignes == null || !lignes.Any())
                throw new ArgumentException("Le bon doit contenir au moins une ligne");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Vérifier que le partenaire existe
                var partenaire = await _context.User.FindAsync(partenaireId);
                if (partenaire == null)
                {
                    _logger.LogWarning("Partenaire introuvable - ID: {PartenaireId}", partenaireId);
                    throw new ArgumentException($"Partenaire avec ID {partenaireId} introuvable");
                }

                // Récupérer le type de document
                var docTypeLibelle = _typeMapping.GetValueOrDefault(typeBon) ?? typeBon;
                var docType = await _context.DocTypes.FirstOrDefaultAsync(d => d.Type == docTypeLibelle);
                if (docType == null)
                {
                    _logger.LogWarning("Type de document introuvable - Type: {DocType}", docTypeLibelle);
                    throw new InvalidOperationException($"Type de document '{docTypeLibelle}' introuvable");
                }

                // Générer le numéro de bon
                var dernierBon = await _context.Bons
                    .Where(b => b.DocType.Type == docTypeLibelle)
                    .OrderByDescending(b => b.Numero)
                    .FirstOrDefaultAsync();

                int nouveauNumero = (dernierBon?.Numero ?? 0) + 1;
                _logger.LogInformation("Nouveau numéro de bon: {Numero}", nouveauNumero);

                // Créer le bon
                var bon = new Bon
                {
                    IdUser = partenaireId,
                    IdDocType = docType.IdDocType,
                    Numero = nouveauNumero,
                    Date = DateTime.Now,
                    LignesBon = new List<LigneBon>()
                };

                // Valider et ajouter les lignes
                foreach (var ligne in lignes)
                {
                    // Validation des données de la ligne
                    if (ligne.IdProduit <= 0)
                        throw new ArgumentException("ID produit invalide");

                    if (ligne.Quantite <= 0)
                        throw new ArgumentException("La quantité doit être positive");

                    if (ligne.PrixUnitaire <= 0)
                        throw new ArgumentException("Le prix unitaire doit être positif");

                    // Vérifier que le produit existe
                    var produit = await _context.Produits
                        .Include(p => p.Categorie)
                        .FirstOrDefaultAsync(p => p.IdProduit == ligne.IdProduit);

                    if (produit == null)
                    {
                        _logger.LogWarning("Produit introuvable - ID: {ProduitId}", ligne.IdProduit);
                        throw new ArgumentException($"Produit avec ID {ligne.IdProduit} introuvable");
                    }

                    // Vérifier le stock pour les sorties
                    if ((typeBon == "BS" || typeBon == "BRF") && ligne.Quantite > produit.QuantiteStock)
                    {
                        _logger.LogWarning("Stock insuffisant - Produit: {ProduitId}, Demandé: {Quantite}, Stock: {Stock}",
                            ligne.IdProduit, ligne.Quantite, produit.QuantiteStock);
                        throw new InvalidOperationException($"Stock insuffisant pour le produit '{produit.Libelle}'. Stock disponible: {produit.QuantiteStock}");
                    }

                    // Créer la ligne
                    var nouvelleLigne = new LigneBon
                    {
                        IdProduit = ligne.IdProduit,
                        Quantite = ligne.Quantite,
                        PrixUnitaire = ligne.PrixUnitaire,
                        Bon = bon
                    };

                    bon.LignesBon.Add(nouvelleLigne);
                    _logger.LogDebug("Ligne ajoutée - Produit: {ProduitId}, Quantité: {Quantite}, Prix: {Prix}",
                        ligne.IdProduit, ligne.Quantite, ligne.PrixUnitaire);
                }

                // Ajouter le bon à la base de données
                _context.Bons.Add(bon);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Bon créé avec succès - ID: {BonId}, Numéro: {Numero}", bon.IdBon, bon.Numero);

                await transaction.CommitAsync();
                return bon;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du bon - Type: {TypeBon}, Partenaire: {PartenaireId}",
                    typeBon, partenaireId);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Bon?> GetBonByIdAsync(int id)
        {
            try
            {
                return await _context.Bons
                    .Include(b => b.Partenaire)
                    .Include(b => b.DocType)
                    .Include(b => b.LignesBon)
                        .ThenInclude(l => l.Produit)
                            .ThenInclude(p => p.Categorie)
                    .FirstOrDefaultAsync(b => b.IdBon == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du bon - ID: {BonId}", id);
                return null;
            }
        }

        public async Task<List<Bon>> GetBonsByTypeAsync(string type)
        {
            try
            {
                var docTypeLibelle = _typeMapping.GetValueOrDefault(type) ?? type;

                return await _context.Bons
                    .Include(b => b.Partenaire)
                    .Include(b => b.DocType)
                    .Include(b => b.LignesBon)
                        .ThenInclude(l => l.Produit)
                    .Where(b => b.DocType.Type == docTypeLibelle)
                    .OrderByDescending(b => b.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des bons - Type: {Type}", type);
                return new List<Bon>();
            }
        }

        public async Task<List<Bon>> GetBonsByTypeAndFiltersAsync(string type, BonFilterViewModel filters)
        {
            try
            {
                var docTypeLibelle = _typeMapping.GetValueOrDefault(type) ?? type;

                var query = _context.Bons
                    .Include(b => b.Partenaire)
                    .Include(b => b.DocType)
                    .Include(b => b.LignesBon)
                        .ThenInclude(l => l.Produit)
                    .Where(b => b.DocType.Type == docTypeLibelle);

                // Appliquer les filtres
                if (filters.PartenaireId.HasValue)
                    query = query.Where(b => b.IdUser == filters.PartenaireId.Value);

                if (filters.DateDebut.HasValue)
                    query = query.Where(b => b.Date >= filters.DateDebut.Value);

                if (filters.DateFin.HasValue)
                    query = query.Where(b => b.Date <= filters.DateFin.Value);

                if (filters.Numero.HasValue)
                    query = query.Where(b => b.Numero == filters.Numero.Value);

                return await query.OrderByDescending(b => b.Date).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des bons filtrés - Type: {Type}", type);
                return new List<Bon>();
            }
        }

        public async Task UpdateStockFromBonAsync(int bonId, string typeBon)
        {
            _logger.LogInformation("Début mise à jour stock - BonId: {BonId}, Type: {TypeBon}", bonId, typeBon);

            var bon = await _context.Bons
                .Include(b => b.LignesBon)
                .Include(b => b.DocType)
                .FirstOrDefaultAsync(b => b.IdBon == bonId);

            if (bon == null)
            {
                _logger.LogWarning("Bon introuvable pour mise à jour stock - ID: {BonId}", bonId);
                throw new ArgumentException($"Bon avec ID {bonId} introuvable");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var ligne in bon.LignesBon)
                {
                    var produit = await _context.Produits.FindAsync(ligne.IdProduit);
                    if (produit != null)
                    {
                        int ancienStock = produit.QuantiteStock;

                        // Logique de mise à jour du stock selon le type de bon
                        switch (typeBon)
                        {
                            case "BE": // Bon d'entrée - augmente le stock
                                produit.QuantiteStock += ligne.Quantite;
                                produit.PrixUnitaire = ligne.PrixUnitaire;
                                break;
                            case "BS": // Bon de sortie - diminue le stock
                                produit.QuantiteStock -= ligne.Quantite;
                                if (produit.QuantiteStock < 0)
                                    produit.QuantiteStock = 0;
                                break;
                            case "BRF": // Bon de retour fournisseur - diminue le stock
                                produit.QuantiteStock -= ligne.Quantite;
                                if (produit.QuantiteStock < 0)
                                    produit.QuantiteStock = 0;
                                break;
                            case "BRC": // Bon de retour client - augmente le stock
                                produit.QuantiteStock += ligne.Quantite;
                                break;
                        }

                        _logger.LogDebug("Stock mis à jour - Produit: {ProduitId}, Ancien: {AncienStock}, Nouveau: {NouveauStock}",
                            produit.IdProduit, ancienStock, produit.QuantiteStock);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Stock mis à jour avec succès - BonId: {BonId}", bonId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du stock - BonId: {BonId}", bonId);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteBonAsync(int id)
        {
            _logger.LogInformation("Début suppression bon - ID: {BonId}", id);

            var bon = await _context.Bons
                .Include(b => b.LignesBon)
                .Include(b => b.DocType)
                .FirstOrDefaultAsync(b => b.IdBon == id);

            if (bon == null)
            {
                _logger.LogWarning("Bon introuvable pour suppression - ID: {BonId}", id);
                return false;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Inverser l'effet sur le stock
                foreach (var ligne in bon.LignesBon)
                {
                    var produit = await _context.Produits.FindAsync(ligne.IdProduit);
                    if (produit != null)
                    {
                        int ancienStock = produit.QuantiteStock;

                        // Inverser selon le type de bon
                        switch (bon.DocType.Type)
                        {
                            case "Entree":
                                produit.QuantiteStock -= ligne.Quantite;
                                if (produit.QuantiteStock < 0)
                                    produit.QuantiteStock = 0;
                                break;
                            case "Sortie":
                                produit.QuantiteStock += ligne.Quantite;
                                break;
                            case "RetourFournisseur":
                                produit.QuantiteStock += ligne.Quantite;
                                break;
                            case "RetourClient":
                                produit.QuantiteStock -= ligne.Quantite;
                                if (produit.QuantiteStock < 0)
                                    produit.QuantiteStock = 0;
                                break;
                        }

                        _logger.LogDebug("Stock inversé - Produit: {ProduitId}, Ancien: {AncienStock}, Nouveau: {NouveauStock}",
                            produit.IdProduit, ancienStock, produit.QuantiteStock);
                    }
                }

                _context.Bons.Remove(bon);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Bon supprimé avec succès - ID: {BonId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du bon - ID: {BonId}", id);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Bon>> GetBonsByPartenaireAsync(int partenaireId, string type)
        {
            try
            {
                var docTypeLibelle = _typeMapping.GetValueOrDefault(type) ?? type;

                return await _context.Bons
                    .Include(b => b.Partenaire)
                    .Include(b => b.DocType)
                    .Include(b => b.LignesBon)
                        .ThenInclude(l => l.Produit)
                    .Where(b => b.DocType.Type == docTypeLibelle && b.IdUser == partenaireId)
                    .OrderByDescending(b => b.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des bons par partenaire - Partenaire: {PartenaireId}, Type: {Type}",
                    partenaireId, type);
                return new List<Bon>();
            }
        }

        public async Task<List<Bon>> GetBonsByDateRangeAsync(DateTime dateDebut, DateTime dateFin, string type)
        {
            try
            {
                var docTypeLibelle = _typeMapping.GetValueOrDefault(type) ?? type;

                return await _context.Bons
                    .Include(b => b.Partenaire)
                    .Include(b => b.DocType)
                    .Include(b => b.LignesBon)
                        .ThenInclude(l => l.Produit)
                    .Where(b => b.DocType.Type == docTypeLibelle &&
                               b.Date >= dateDebut && b.Date <= dateFin)
                    .OrderByDescending(b => b.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des bons par date - Du: {DateDebut} Au: {DateFin}, Type: {Type}",
                    dateDebut, dateFin, type);
                return new List<Bon>();
            }
        }

        public async Task<BonStatsViewModel> GetBonStatsAsync(string type)
        {
            try
            {
                var docTypeLibelle = _typeMapping.GetValueOrDefault(type) ?? type;

                var bons = await _context.Bons
                    .Include(b => b.LignesBon)
                    .Where(b => b.DocType.Type == docTypeLibelle)
                    .ToListAsync();

                return new BonStatsViewModel
                {
                    NombreBons = bons.Count,
                    MontantTotal = bons.Sum(b => b.CalculerTotal()),
                    QuantiteTotale = bons.Sum(b => b.LignesBon.Sum(l => l.Quantite)),
                    MoyenneParBon = bons.Any() ? bons.Average(b => b.CalculerTotal()) : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du calcul des statistiques - Type: {Type}", type);
                return new BonStatsViewModel();
            }
        }
    }
}