using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels.gestiondesReglements;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Services
{
    public class ReglementService : IReglementService
    {

        private readonly ApplicationDbContext _context;

        public ReglementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReglementIndexViewModel> GetReglementIndexViewModelAsync(ReglementFilters filters)
        {
            var query = _context.Set<Reglement>()
                .Include(r => r.Partenaire)
                .AsQueryable();

            // Application des filtres
            if (filters.PartenaireId.HasValue)
                query = query.Where(r => r.IdUser == filters.PartenaireId.Value);

            if (filters.DateDebut.HasValue)
                query = query.Where(r => r.DatePaiement >= filters.DateDebut.Value);

            if (filters.DateFin.HasValue)
                query = query.Where(r => r.DatePaiement <= filters.DateFin.Value);

            if (filters.ModePaiement.HasValue)
                query = query.Where(r => r.ModePaiement == filters.ModePaiement.Value);

            if (filters.MontantMin.HasValue)
                query = query.Where(r => r.Montant >= filters.MontantMin.Value);

            if (filters.MontantMax.HasValue)
                query = query.Where(r => r.Montant <= filters.MontantMax.Value);

            var reglements = await query
                .OrderByDescending(r => r.DatePaiement)
                .ToListAsync();

            var partenaires = await _context.User
                .OrderBy(p => p.Nom)
                .ToListAsync();

            var stats = await GetStatsAsync(filters);

            return new ReglementIndexViewModel
            {
                Reglements = reglements,
                Partenaires = partenaires,
                Filters = filters,
                Stats = stats,
                TitrePage = "Règlements",
                TypePartenaire = "Partenaire"
            };
        }

        public async Task<ReglementCreateEditViewModel> GetCreateViewModelAsync()
        {
            var partenaires = await _context.User
                .OrderBy(p => p.Nom)
                .ToListAsync();

            return new ReglementCreateEditViewModel
            {
                Partenaires = partenaires,
                ActionTitle = "Nouveau règlement",
                IsEdit = false,
                Reglement = new Reglement
                {
                    DatePaiement = DateTime.Today
                }
            };
        }

        public async Task<ReglementCreateEditViewModel> GetEditViewModelAsync(int id)
        {
            var reglement = await GetReglementByIdAsync(id);
            if (reglement == null)
                return null;

            var partenaires = await _context.User
                .OrderBy(p => p.Nom)
                .ToListAsync();

            return new ReglementCreateEditViewModel
            {
                Reglement = reglement,
                Partenaires = partenaires,
                ActionTitle = "Modifier le règlement",
                IsEdit = true
            };
        }

        public async Task<ReglementDetailsViewModel> GetDetailsViewModelAsync(int id)
        {
            var reglement = await _context.Set<Reglement>()
                .Include(r => r.Partenaire)
                .FirstOrDefaultAsync(r => r.IdReglement == id);

            if (reglement == null)
                return null;

            return new ReglementDetailsViewModel
            {
                Reglement = reglement,
                TypePartenaire = reglement.Partenaire is Client ? "Client" : "Fournisseur"
            };
        }

        public async Task<bool> CreateReglementAsync(Reglement reglement)
        {
            try
            {
                reglement.DateCreation = DateTime.Now;
                _context.Set<Reglement>().Add(reglement);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateReglementAsync(Reglement reglement)
        {
            try
            {
                _context.Set<Reglement>().Update(reglement);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteReglementAsync(int id)
        {
            try
            {
                var reglement = await GetReglementByIdAsync(id);
                if (reglement == null)
                    return false;

                _context.Set<Reglement>().Remove(reglement);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Reglement?> GetReglementByIdAsync(int id)
        {
            return await _context.Set<Reglement>()
                .Include(r => r.Partenaire)
                .FirstOrDefaultAsync(r => r.IdReglement == id);
        }

        public async Task<List<Reglement>> GetReglementsByPartenaireAsync(int partenaireId)
        {
            return await _context.Set<Reglement>()
                .Include(r => r.Partenaire)
                .Where(r => r.IdUser == partenaireId)
                .OrderByDescending(r => r.DatePaiement)
                .ToListAsync();
        }

        public async Task<ReglementStats> GetStatsAsync(ReglementFilters filters)
        {
            var query = _context.Set<Reglement>().AsQueryable();

            // Application des mêmes filtres que pour la liste
            if (filters.PartenaireId.HasValue)
                query = query.Where(r => r.IdUser == filters.PartenaireId.Value);

            if (filters.DateDebut.HasValue)
                query = query.Where(r => r.DatePaiement >= filters.DateDebut.Value);

            if (filters.DateFin.HasValue)
                query = query.Where(r => r.DatePaiement <= filters.DateFin.Value);

            if (filters.ModePaiement.HasValue)
                query = query.Where(r => r.ModePaiement == filters.ModePaiement.Value);

            if (filters.MontantMin.HasValue)
                query = query.Where(r => r.Montant >= filters.MontantMin.Value);

            if (filters.MontantMax.HasValue)
                query = query.Where(r => r.Montant <= filters.MontantMax.Value);

            var reglements = await query.ToListAsync();

            var stats = new ReglementStats
            {
                NombreReglements = reglements.Count,
                MontantTotal = reglements.Sum(r => r.Montant),
                MoyenneParReglement = reglements.Count > 0 ? reglements.Average(r => r.Montant) : 0
            };

            // Répartition par mode de paiement
            stats.RepartitionParMode = reglements
                .GroupBy(r => r.ModePaiement)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Montant));

            return stats;
        }
    }
}
 
