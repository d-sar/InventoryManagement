using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Services
{
    public class FournisseurService: IFournisseurService
    {
        private readonly ApplicationDbContext _context;

        public FournisseurService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Fournisseur>> GetAllFournisseursAsync()
        {
            return await _context.Fournisseurs.Include(f => f.Bons).ToListAsync();
        }

        public async Task<Fournisseur?> GetFournisseurByIdAsync(int id)
        {
            return await _context.Fournisseurs.Include(f => f.Bons)
                .ThenInclude(b => b.LignesBon)
                .ThenInclude(l => l.Produit)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddFournisseurAsync(Fournisseur fournisseur)
        {
            _context.Fournisseurs.Add(fournisseur);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFournisseurAsync(Fournisseur fournisseur)
        {
            _context.Fournisseurs.Update(fournisseur);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFournisseurAsync(int id)
        {
            var fournisseur = await _context.Fournisseurs.FindAsync(id);
            if (fournisseur != null)
            {
                _context.Fournisseurs.Remove(fournisseur);
                await _context.SaveChangesAsync();
            }
        }
    }
}
