// Program.cs
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.ReposDapper;
using Spotify.DTOs;
using Scalar.AspNetCore;
using System.Data;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
var connectionString = builder.Configuration.GetConnectionString("MySQL");
builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection(connectionString));

// Repositorios
builder.Services.AddScoped<IRepoArtista, RepoArtista>();
builder.Services.AddScoped<IRepoAlbum, RepoAlbum>();
builder.Services.AddScoped<IRepoUsuario, RepoUsuario>();
builder.Services.AddScoped<IRepoGenero, RepoGenero>();
builder.Services.AddScoped<IRepoCancion, RepoCancion>();
builder.Services.AddScoped<IRepoPlaylist, RepoPlaylist>();
builder.Services.AddScoped<IRepoNacionalidad, RepoNacionalidad>();
builder.Services.AddScoped<IRepoReproduccion, RepoReproduccion>();
builder.Services.AddScoped<IRepoTipoSuscripcion, RepoTipoSuscripcion>();
builder.Services.AddScoped<IRepoRegistro, RepoSuscripcion>();

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