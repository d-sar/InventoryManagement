namespace InventoryManagementMVC.Models.Entities
{
    public class Client : User
    {

        public DateTime DateInscription { get; set; } = DateTime.Now;
        public bool EstActif { get; set; } = true;

        public Client()
        {
            Type = "Client";
        }
    }
}
