using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Services
{
    public class CategorieService : ICategorieService
    {
        private readonly ApplicationDbContext _context;

        public CategorieService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categorie>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.Produits.Where(p => p.IsActive))
                .OrderBy(c => c.Nom)
                .ToListAsync();
        }

        public async Task<Categorie?> GetCategorieByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Produits.Where(p => p.IsActive))
                .FirstOrDefaultAsync(c => c.IdCategorie == id);
        }

        public async Task<Categorie> CreateCategorieAsync(CreateCategorieViewModel model)
        {
            // Vérifier si une catégorie avec le même nom existe déjà
            var existingCategorie = await _context.Categories
                .FirstOrDefaultAsync(c => c.Nom.ToLower() == model.Nom.ToLower());

            if (existingCategorie != null)
            {
                throw new InvalidOperationException("Une catégorie avec ce nom existe déjà.");
            }

            var categorie = new Categorie
            {
                Nom = model.Nom.Trim(),
                Description = model.Description?.Trim() ?? string.Empty
            };

            _context.Categories.Add(categorie);
            await _context.SaveChangesAsync();

            return categorie;
        }

        public async Task<bool> UpdateCategorieAsync(int id, CreateCategorieViewModel model)
        {
            var categorie = await _context.Categories.FindAsync(id);
            if (categorie == null)
                return false;

            // Vérifier si une autre catégorie avec le même nom existe déjà
            var existingCategorie = await _context.Categories
                .FirstOrDefaultAsync(c => c.Nom.ToLower() == model.Nom.ToLower() && c.IdCategorie != id);

            if (existingCategorie != null)
            {
                throw new InvalidOperationException("Une catégorie avec ce nom existe déjà.");
            }

            categorie.Nom = model.Nom.Trim();
            categorie.Description = model.Description?.Trim() ?? string.Empty;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategorieAsync(int id)
        {
            var categorie = await _context.Categories
                .Include(c => c.Produits.Where(p => p.IsActive))
                .FirstOrDefaultAsync(c => c.IdCategorie == id);

            if (categorie == null)
                return false;

            // Vérifier si la catégorie a des produits actifs
            if (categorie.Produits.Any())
            {
                throw new InvalidOperationException("Impossible de supprimer cette catégorie car elle contient des produits actifs.");
            }

            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Categorie>> SearchCategoriesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllCategoriesAsync();

            return await _context.Categories
                .Include(c => c.Produits.Where(p => p.IsActive))
                .Where(c => c.Nom.Contains(searchTerm) ||
                           c.Description.Contains(searchTerm))
                .OrderBy(c => c.Nom)
                .ToListAsync();
        }

        public async Task<bool> CategorieHasProduitsAsync(int categorieId)
        {
            return await _context.Produits
                .AnyAsync(p => p.IdCategorie == categorieId && p.IsActive);
        }

        public async Task<int> GetProduitsCountByCategorieAsync(int categorieId)
        {
            return await _context.Produits
                .CountAsync(p => p.IdCategorie == categorieId && p.IsActive);
        }
    }
    }
