using InventoryManagementMVC.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Client> Clients { get; set; }
        public DbSet<Fournisseur> Fournisseurs { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<Bon> Bons { get; set; }
        public DbSet<DocType> DocTypes { get; set; }
        public DbSet<LigneBon> LigneBons { get; set; }

        //public DbSet<BonEntree> BonsEntree { get; set; }
        // public DbSet<BonRetourClient> BonsRetourClient { get; set; }
        // public DbSet<BonRetourFournisseur> BonsRetourFournisseur { get; set; }
        public DbSet<LigneBon> LignesBon { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration TPH (Table Per Hierarchy) pour User
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("Type")
                .HasValue<Client>("Client")
                .HasValue<Fournisseur>("Fournisseur");


            // Relations

            // Relation Partenaire -> Bon (Un partenaire peut avoir plusieurs bons)
            modelBuilder.Entity<Bon>()
                .HasOne(b => b.Partenaire)
                .WithMany(p => p.Bons)
                .HasForeignKey(b => b.IdUser)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation DocType -> Bon (Un type de document peut classifier plusieurs bons)
            modelBuilder.Entity<Bon>()
                .HasOne(b => b.DocType)
                .WithMany(dt => dt.Bons)
                .HasForeignKey(b => b.IdDocType)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Bon -> LigneBon (Un bon contient plusieurs lignes)
            modelBuilder.Entity<LigneBon>()
                .HasOne(l => l.Bon)
                .WithMany(b => b.LignesBon)
                .HasForeignKey(l => l.IdBon)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Produit -> LigneBon (Un produit peut être référencé dans plusieurs lignes)
            modelBuilder.Entity<LigneBon>()
                .HasOne(l => l.Produit)
                .WithMany(p => p.LignesBon)
                .HasForeignKey(l => l.IdProduit)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Categorie -> Produit (Une catégorie classifie plusieurs produits)
            modelBuilder.Entity<Produit>()
                .HasOne(p => p.Categorie)
                .WithMany(c => c.Produits)
                .HasForeignKey(p => p.IdCategorie)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Categorie>()
                .HasMany(c => c.Produits)
                .WithOne(p => p.Categorie)
                .HasForeignKey(p => p.IdCategorie);

            // Index pour performance
            modelBuilder.Entity<Produit>()
                .HasIndex(p => p.Libelle);

            modelBuilder.Entity<Produit>()
                .HasIndex(p => p.IdCategorie);




        }
    }
}
