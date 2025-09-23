using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.ReposDapper;
using MinimalAPI.DTOs;
using Scalar.AspNetCore;
using System.Data;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
var connectionString = builder.Configuration.GetConnectionString("MySQL");
builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection(connectionString));

builder.Services.AddScoped<IRepoArtista, RepoArtista>();
builder.Services.AddScoped<IRepoAlbum, RepoAlbum>();
builder.Services.AddScoped<IRepoUsuario, RepoUsuario>();
builder.Services.AddScoped<IRepoGenero, RepoGenero>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });
    app.MapScalarApiReference();
}

// ENTIDADES CON DTO (Usuario, Genero)

// ---------- Usuario ----------
app.MapGet("/usuarios", (IRepoUsuario repo) =>
{
    var usuarios = repo.Obtener();
    return Results.Ok(usuarios.Select(u => new UsuarioOutputDTO
    {
        idUsuario = u.idUsuario,
        NombreUsuario = u.NombreUsuario,
        Gmail = u.Gmail,
        Nacionalidad = u.nacionalidad?.Pais ?? "Desconocida"
    }));
});

app.MapGet("/usuarios/{id}", (IRepoUsuario repo, uint id) =>
{
    var usuario = repo.DetalleDe(id);
    if (usuario is null) return Results.NotFound();

    return Results.Ok(new UsuarioOutputDTO
    {
        idUsuario = usuario.idUsuario,
        NombreUsuario = usuario.NombreUsuario,
        Gmail = usuario.Gmail,
        Nacionalidad = usuario.nacionalidad?.Pais ?? "Desconocida"
    });
});

app.MapPost("/usuarios", (IRepoUsuario repo, UsuarioInputDTO dto) =>
{
    var usuario = new Usuario
    {
        NombreUsuario = dto.NombreUsuario,
        Gmail = dto.Gmail,
        Contrasenia = dto.Contrasenia,
        nacionalidad = new Nacionalidad 
        { 
            idNacionalidad = dto.Nacionalidad,
            Pais = string.Empty  // <-- agregado
        }
    };

    repo.Alta(usuario);

    return Results.Created($"/usuarios/{usuario.idUsuario}", new UsuarioOutputDTO
    {
        idUsuario = usuario.idUsuario,
        NombreUsuario = usuario.NombreUsuario,
        Gmail = usuario.Gmail,
        Nacionalidad = usuario.nacionalidad?.Pais ?? "Desconocida"
    });
});

// ---------- Genero ----------
app.MapGet("/generos", (IRepoGenero repo) =>
{
    var generos = repo.Obtener();
    return Results.Ok(generos.Select(g => new GeneroOutputDTO
    {
        idGenero = g.idGenero,
        genero = g.genero
    }));
});

app.MapGet("/generos/{id}", (IRepoGenero repo, byte id) =>
{
    var genero = repo.DetalleDe(id);
    if (genero is null) return Results.NotFound();

    return Results.Ok(new GeneroOutputDTO
    {
        idGenero = genero.idGenero,
        genero = genero.genero
    });
});

app.MapPost("/generos", (IRepoGenero repo, GeneroInputDTO dto) =>
{
    var genero = new Genero { genero = dto.genero };
    repo.Alta(genero);

    return Results.Created($"/generos/{genero.idGenero}", new GeneroOutputDTO
    {
        idGenero = genero.idGenero,
        genero = genero.genero
    });
});

// ENTIDADES SIN DTO (Artista, Album)

// ---------- Artista ----------
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

// ---------- Album ----------
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