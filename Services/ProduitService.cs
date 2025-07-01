using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using InventoryManagementMVC.Models.ViewModels;
using InventoryManagementMVC.Services;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Services
{
    public class ProduitService : IProduitService
    {
            private readonly ApplicationDbContext _context;

            public ProduitService(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Produit>> GetAllProduitsAsync()
            {
                return await _context.Produits
                    .Include(p => p.Categorie)
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Libelle)
                    .ToListAsync();
            }

            public async Task<Produit?> GetProduitByIdAsync(int id)
            {
                return await _context.Produits
                    .Include(p => p.Categorie)
                    .FirstOrDefaultAsync(p => p.IdProduit == id && p.IsActive);
            }

            public async Task<Produit> CreateProduitAsync(CreateProduitViewModel model)
            {
                var produit = new Produit
                {
                    IdCategorie = model.IdCategorie,
                    Libelle = model.Libelle,
                    Description = model.Description,
                    PrixUnitaire = model.PrixUnitaire,
                    QuantiteStock = model.QuantiteStock,
                    
                    IsActive = true
                };

                _context.Produits.Add(produit);
                await _context.SaveChangesAsync();

                return produit;
            }

            public async Task<bool> UpdateProduitAsync(int id, CreateProduitViewModel model)
            {
                var produit = await _context.Produits.FindAsync(id);
                if (produit == null || !produit.IsActive)
                    return false;

                produit.IdCategorie = model.IdCategorie;
                produit.Libelle = model.Libelle;
                produit.Description = model.Description;
                produit.PrixUnitaire = model.PrixUnitaire;
                produit.QuantiteStock = model.QuantiteStock;

                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<bool> DeleteProduitAsync(int id)
            {
                var produit = await _context.Produits.FindAsync(id);
                if (produit == null)
                    return false;

                // Soft delete
                produit.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<IEnumerable<Produit>> SearchProduitsAsync(string searchTerm)
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetAllProduitsAsync();

                return await _context.Produits
                    .Include(p => p.Categorie)
                    .Where(p => p.IsActive &&
                           (p.Libelle.Contains(searchTerm) ||
                            p.Description.Contains(searchTerm)))
                    .OrderBy(p => p.Libelle)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Produit>> GetProduitsByCategorieAsync(int categorieId)
            {
                return await _context.Produits
                    .Include(p => p.Categorie)
                    .Where(p => p.IdCategorie == categorieId && p.IsActive)
                    .OrderBy(p => p.Libelle)
                    .ToListAsync();
            }

            public async Task<bool> UpdateStockAsync(int produitId, int nouvelleQuantite)
            {
                var produit = await _context.Produits.FindAsync(produitId);
                if (produit == null || !produit.IsActive)
                    return false;

                produit.QuantiteStock = nouvelleQuantite;
                await _context.SaveChangesAsync();
                return true;
            }
    }
}

