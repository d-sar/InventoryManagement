using InventoryManagementMVC.Models.Entities;

namespace InventoryManagementMVC.Services
{
    public interface  IClientService
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<Client?> GetClientByIdAsync(int id);
        Task<Client> AddClientAsync(Client client);
        Task<Client> UpdateClientAsync(Client client);
        Task<bool> DeleteClientAsync(int id);
        Task<IEnumerable<Client>> GetActiveClientsAsync();
        Task<IEnumerable<Client>> SearchClientsAsync(string searchTerm);
        Task<bool> ClientExistsAsync(int id);
        Task<decimal> GetClientTotalPurchasesAsync(int clientId);
    }
}
