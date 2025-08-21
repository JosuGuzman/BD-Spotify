using Spotify.Core;
using Scalar.AspNetCore;
using Spotify.ReposDapper;
using System.Data;
using MySqlConnector;
using Spotify.Core.Persistencia;
using MinimalAPI;

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
    app.MapScalarApiReference();
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

// Endpoints Usuario con DTO
app.MapGet("/usuarios", (IRepoUsuario repo) =>
{
    var usuarios = repo.Obtener();
    return Results.Ok(usuarios.Select(u => new UsuarioOutputDTO
    {
        idUsuario = u.idUsuario,
        NombreUsuario = u.NombreUsuario,
        Gmail = u.Gmail,
        Nacionalidad = u.nacionalidad.Pais
    }));
});

app.MapGet("/usuarios/{id}", (IRepoUsuario repo, uint id) =>
{
    var usuario = repo.DetalleDe(id);

    if (usuario is null)
        return Results.NotFound();

    return Results.Ok(new UsuarioOutputDTO
    {
        idUsuario = usuario.idUsuario,
        NombreUsuario = usuario.NombreUsuario,
        Gmail = usuario.Gmail,
        Nacionalidad = usuario.nacionalidad.Pais
    });
});

app.MapPost("/usuarios", (IRepoUsuario repo, UsuarioInputDTO usuarioDto) =>
{
    var usuario = new Usuario
    {
        NombreUsuario = usuarioDto.NombreUsuario,
        Gmail = usuarioDto.Gmail,
        Contrasenia = usuarioDto.Contrasenia,
        nacionalidad = new Nacionalidad { Pais = usuarioDto.Nacionalidad }
    };

    var id = repo.Alta(usuario);

    return Results.Created($"/usuarios/{id}", new UsuarioOutputDTO
    {
        idUsuario = id,
        NombreUsuario = usuario.NombreUsuario,
        Gmail = usuario.Gmail,
        Nacionalidad = usuario.nacionalidad.Pais
    });
});

// Endpoints Canción
app.MapGet("/canciones", (IRepoCancion repo) =>
{
    var canciones = repo.Obtener();
    return Results.Ok(canciones.Select(c => new CancionOutputDTO
    {
        idCancion = c.idCancion,
        Titulo = c.Titulo,
        Duracion = c.Duracion,
        Artista = c.artista.NombreArtistico
    }));
});

app.MapGet("/canciones/{id}", (IRepoCancion repo, uint id) =>
{
    var cancion = repo.DetalleDe(id);

    if (cancion is null)
        return Results.NotFound();

    return Results.Ok(new CancionOutputDTO
    {
        idCancion = cancion.idCancion,
        Titulo = cancion.Titulo,
        Duracion = cancion.Duracion,
        Artista = cancion.artista.NombreArtistico
    });
});

app.Run();