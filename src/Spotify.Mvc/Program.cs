
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.ReposDapper;
using System.Data;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Obtener cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("MySQL");

// Registrar IDbConnection
builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection(connectionString));

// Registrar repositorios
builder.Services.AddScoped<IRepoArtista, RepoArtista>();
builder.Services.AddScoped<IRepoAlbum, RepoAlbum>();
builder.Services.AddScoped<IRepoUsuario, RepoUsuario>();
builder.Services.AddScoped<IRepoGenero, RepoGenero>();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();