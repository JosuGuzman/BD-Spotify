// Program.cs
using Spotify.Core.Persistencia;
using Spotify.ReposDapper;
using Scalar.AspNetCore;
using System.Data;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
var connectionString = builder.Configuration.GetConnectionString("MySQL");
builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection(connectionString));

// REGISTRO COMPLETO DE REPOSITORIOS (SÍNCRONOS Y ASÍNCRONOS)
builder.Services.AddScoped<IRepoAlbum, RepoAlbum>();
builder.Services.AddScoped<IRepoAlbumAsync, RepoAlbumAsync>();

builder.Services.AddScoped<IRepoArtista, RepoArtista>();
builder.Services.AddScoped<IRepoArtistaAsync, RepoArtistaAsync>();

builder.Services.AddScoped<IRepoCancion, RepoCancion>();
builder.Services.AddScoped<IRepoCancionAsync, RepoCancionAsync>();

builder.Services.AddScoped<IRepoGenero, RepoGenero>();
builder.Services.AddScoped<IRepoGeneroAsync, RepoGeneroAsync>();

builder.Services.AddScoped<IRepoUsuario, RepoUsuario>();
builder.Services.AddScoped<IRepoUsuarioAsync, RepoUsuarioAsync>();

builder.Services.AddScoped<IRepoNacionalidad, RepoNacionalidad>();
builder.Services.AddScoped<IRepoNacionalidadAsync, RepoNacionalidadAsync>();

// MVC
builder.Services.AddControllersWithViews();

// Swagger para API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// MVC Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Swagger para desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });
    app.MapScalarApiReference();
}

app.Run();