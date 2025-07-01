using InventoryManagementMVC.Data;
using InventoryManagementMVC.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Services
{

    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;

        public ClientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _context.Clients
                .Include(c => c.Bons)
                    .ThenInclude(b => b.LignesBon)
                .OrderBy(c => c.Nom)
                .ToListAsync();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.Bons)
                    .ThenInclude(b => b.LignesBon)
                        .ThenInclude(l => l.Produit)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Client> AddClientAsync(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            client.Type = "Client";
            //client.DateInscription = DateTime.Now;
            client.EstActif = true;

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<Client> UpdateClientAsync(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return false;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Client>> GetActiveClientsAsync()
        {
            return await _context.Clients
                .Where(c => c.EstActif)
                .Include(c => c.Bons)
                .OrderBy(c => c.Nom)
                .ToListAsync();
        }

        public async Task<IEnumerable<Client>> SearchClientsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllClientsAsync();

            return await _context.Clients
                .Where(c => c.Nom.Contains(searchTerm) ||
                           c.Email.Contains(searchTerm) ||
                           c.Telephone.Contains(searchTerm))
                .Include(c => c.Bons)
                .OrderBy(c => c.Nom)
                .ToListAsync();
        }

        public async Task<bool> ClientExistsAsync(int id)
        {
            return await _context.Clients.AnyAsync(c => c.Id == id);
        }

        public async Task<decimal> GetClientTotalPurchasesAsync(int clientId)
        {
            return await _context.Clients
                .Where(c => c.Id == clientId)
                .SelectMany(c => c.Bons)
                .SelectMany(b => b.LignesBon)
                .SumAsync(l => l.Quantite * l.PrixUnitaire);
        }
    }
}
