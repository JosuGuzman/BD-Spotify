using Spotify.Core;
using Spotify.ReposDapper;
using System.Data;
using MySqlConnector;
using Spotify.Core.Persistencia;

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("MySQL");

// Registrar IDbConnection para inyección de dependencias
builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection(connectionString));

// Registrar repositorios
builder.Services.AddScoped<IRepoArtista,RepoArtista>();
builder.Services.AddScoped<IRepoAlbum,RepoAlbum>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Spotify API v1");
        options.RoutePrefix = "swagger";
    });
}

// Endpoints Artista
app.MapGet("/artistas", (IRepoArtista repo) => repo.Obtener());

app.MapGet("/artistas/{id}", (IRepoArtista repo, uint id) =>
{
    var artista = repo.DetalleDe(id);
    return artista is not null ? Results.Ok(artista) : Results.NotFound();
});

app.MapPost("/artistas", (IRepoArtista repo, Artista artista) =>
{
    var id = repo.Alta(artista);
    return Results.Created($"/artistas/{id}", artista);
});

// Endpoints Álbum
app.MapGet("/albumes", (IRepoAlbum repo) => repo.Obtener());

app.MapGet("/albumes/{id}", (IRepoAlbum repo, uint id) =>
{
    var album = repo.DetalleDe(id);
    return album is not null ? Results.Ok(album) : Results.NotFound();
});

app.MapPost("/albumes", (IRepoAlbum repo, Album album) =>
{
    var id = repo.Alta(album);
    return Results.Created($"/albumes/{id}", album);
});

app.Run();