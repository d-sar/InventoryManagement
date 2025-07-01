
using InventoryManagementMVC.Data;
using InventoryManagementMVC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)) // adapte selon ta version MySQL
    ));

// Enregistrement des services
builder.Services.AddScoped<IProduitService, ProduitService>();
builder.Services.AddScoped<ICategorieService, CategorieService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IFournisseurService, FournisseurService>();

// Configuration MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuration du pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();