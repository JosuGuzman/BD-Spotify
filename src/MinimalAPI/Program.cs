using Spotify.ReposDapper;
using Spotify.Core.Persistencia;

var builder = WebApplication.CreateBuilder(args);

// === CONFIGURACIÓN DE SERVICIOS ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Spotify API", Version = "v1" });
});

// Database Factory
builder.Services.AddScoped<IConnectionFactory>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string not found");
    return new DbConnectionFactory(connectionString);
});

// Repositories
builder.Services.AddScoped<IRepoAlbum, RepoAlbum>();
builder.Services.AddScoped<IRepoArtista, RepoArtista>();
builder.Services.AddScoped<IRepoCancion, RepoCancion>();
builder.Services.AddScoped<IRepoGenero, RepoGenero>();
builder.Services.AddScoped<IRepoPlaylist, RepoPlaylist>();
builder.Services.AddScoped<IRepoReproduccion, RepoReproduccion>();
builder.Services.AddScoped<IRepoUsuario, RepoUsuario>();
builder.Services.AddScoped<IRepoNacionalidad, RepoNacionalidad>();
builder.Services.AddScoped<IRepoTipoSuscripcion, RepoTipoSuscripcion>();
builder.Services.AddScoped<IRepoRegistro, RepoSuscripcion>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// === CONFIGURACIÓN DEL PIPELINE ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

// Health Check
app.MapHealthChecks("/health");

// Global Exception Handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "Internal server error", details = ex.Message });
    }
});

// === ENDPOINTS DE ALBUM ===
var albumsGroup = app.MapGroup("/albums").WithTags("Albums");

albumsGroup.MapGet("/", async (IRepoAlbum repo, [FromQuery] int page = 1, [FromQuery] int size = 20) =>
{
    var albums = await repo.ObtenerPaginadoAsync(page, size);
    return Results.Ok(new { data = albums, page, size });
})
.WithName("GetAlbums")
.WithOpenApi();

albumsGroup.MapGet("/{id}", async (uint id, IRepoAlbum repo) =>
{
    var album = await repo.ObtenerPorIdAsync(id);
    return album is not null ? Results.Ok(album) : Results.NotFound(new { error = "Album no encontrado" });
})
.WithName("GetAlbumById")
.WithOpenApi();

albumsGroup.MapGet("/recent", async (IRepoAlbum repo, [FromQuery] int limit = 10) =>
{
    var albums = await repo.ObtenerAlbumesRecientesAsync(limit);
    return Results.Ok(albums);
})
.WithName("GetRecentAlbums")
.WithOpenApi();

albumsGroup.MapGet("/artist/{artistId}", async (uint artistId, IRepoAlbum repo) =>
{
    var albums = await repo.ObtenerPorArtistaAsync(artistId);
    return Results.Ok(albums);
})
.WithName("GetAlbumsByArtist")
.WithOpenApi();

albumsGroup.MapPost("/", async ([FromBody] AlbumCreateRequest request, IRepoAlbum repo) =>
{
    var album = new Album
    {
        Titulo = request.Titulo,
        Artista = new Artista { idArtista = request.ArtistaId },
        Portada = request.Portada,
        FechaLanzamiento = request.FechaLanzamiento
    };
    
    await repo.InsertarAsync(album);
    return Results.Created($"/albums/{album.idAlbum}", new { id = album.idAlbum, message = "Album creado exitosamente" });
})
.WithName("CreateAlbum")
.WithOpenApi();

albumsGroup.MapPut("/{id}", async (uint id, [FromBody] AlbumUpdateRequest request, IRepoAlbum repo) =>
{
    var existing = await repo.ObtenerPorIdAsync(id);
    if (existing is null) return Results.NotFound(new { error = "Album no encontrado" });
    
    existing.Titulo = request.Titulo ?? existing.Titulo;
    existing.Portada = request.Portada ?? existing.Portada;
    existing.FechaLanzamiento = request.FechaLanzamiento ?? existing.FechaLanzamiento;
    
    if (request.ArtistaId.HasValue)
        existing.Artista.idArtista = request.ArtistaId.Value;
    
    await repo.ActualizarAsync(existing);
    return Results.Ok(new { message = "Album actualizado exitosamente" });
})
.WithName("UpdateAlbum")
.WithOpenApi();

albumsGroup.MapDelete("/{id}", async (uint id, IRepoAlbum repo) =>
{
    var existing = await repo.ObtenerPorIdAsync(id);
    if (existing is null) return Results.NotFound(new { error = "Album no encontrado" });
    
    await repo.EliminarAsync(id);
    return Results.Ok(new { message = "Album eliminado exitosamente" });
})
.WithName("DeleteAlbum")
.WithOpenApi();

// === ENDPOINTS DE ARTISTA ===
var artistsGroup = app.MapGroup("/artists").WithTags("Artists");

artistsGroup.MapGet("/", async (IRepoArtista repo, [FromQuery] int page = 1, [FromQuery] int size = 20) =>
{
    var artists = await repo.ObtenerPaginadoAsync(page, size);
    return Results.Ok(new { data = artists, page, size });
})
.WithName("GetArtists")
.WithOpenApi();

artistsGroup.MapGet("/{id}", async (uint id, IRepoArtista repo) =>
{
    var artist = await repo.ObtenerPorIdAsync(id);
    return artist is not null ? Results.Ok(artist) : Results.NotFound(new { error = "Artista no encontrado" });
})
.WithName("GetArtistById")
.WithOpenApi();

artistsGroup.MapGet("/popular", async (IRepoArtista repo, [FromQuery] int limit = 10) =>
{
    var artists = await repo.ObtenerArtistasPopularesAsync(limit);
    return Results.Ok(artists);
})
.WithName("GetPopularArtists")
.WithOpenApi();

artistsGroup.MapGet("/search/{term}", async (string term, IRepoArtista repo) =>
{
    var artists = await repo.BuscarTextoAsync(term);
    return Results.Ok(artists);
})
.WithName("SearchArtists")
.WithOpenApi();

artistsGroup.MapPost("/", async ([FromBody] ArtistaCreateRequest request, IRepoArtista repo) =>
{
    var artista = new Artista
    {
        NombreArtistico = request.NombreArtistico,
        Nombre = request.Nombre,
        Apellido = request.Apellido
    };
    
    await repo.InsertarAsync(artista);
    return Results.Created($"/artists/{artista.idArtista}", new { id = artista.idArtista, message = "Artista creado exitosamente" });
})
.WithName("CreateArtist")
.WithOpenApi();

// === ENDPOINTS DE CANCIÓN ===
var songsGroup = app.MapGroup("/songs").WithTags("Songs");

songsGroup.MapGet("/", async (IRepoCancion repo, [FromQuery] int page = 1, [FromQuery] int size = 20) =>
{
    var songs = await repo.ObtenerPaginadoAsync(page, size);
    return Results.Ok(new { data = songs, page, size });
})
.WithName("GetSongs")
.WithOpenApi();

songsGroup.MapGet("/{id}", async (uint id, IRepoCancion repo) =>
{
    var song = await repo.ObtenerPorIdAsync(id);
    return song is not null ? Results.Ok(song) : Results.NotFound(new { error = "Canción no encontrada" });
})
.WithName("GetSongById")
.WithOpenApi();

songsGroup.MapGet("/popular", async (IRepoCancion repo, [FromQuery] int limit = 10) =>
{
    var songs = await repo.ObtenerCancionesPopularesAsync(limit);
    return Results.Ok(songs);
})
.WithName("GetPopularSongs")
.WithOpenApi();

songsGroup.MapGet("/album/{albumId}", async (uint albumId, IRepoCancion repo) =>
{
    var songs = await repo.ObtenerPorAlbumAsync(albumId);
    return Results.Ok(songs);
})
.WithName("GetSongsByAlbum")
.WithOpenApi();

songsGroup.MapGet("/artist/{artistId}", async (uint artistId, IRepoCancion repo) =>
{
    var songs = await repo.ObtenerPorArtistaAsync(artistId);
    return Results.Ok(songs);
})
.WithName("GetSongsByArtist")
.WithOpenApi();

songsGroup.MapGet("/genre/{genreId}", async (byte genreId, IRepoCancion repo) =>
{
    var songs = await repo.ObtenerPorGeneroAsync(genreId);
    return Results.Ok(songs);
})
.WithName("GetSongsByGenre")
.WithOpenApi();

songsGroup.MapPost("/{id}/play", async (uint id, IRepoCancion repo, IRepoReproduccion repoRep) =>
{
    // Simulación de usuario autenticado (en producción usar JWT)
    uint usuarioId = 1;
    await repo.IncrementarReproduccionesAsync(id);
    await repoRep.RegistrarReproduccionAsync(usuarioId, id, TimeSpan.Zero, true, "Web API");
    
    return Results.Ok(new { message = "Reproducción registrada exitosamente" });
})
.WithName("PlaySong")
.WithOpenApi();

// === ENDPOINTS DE USUARIO ===
var usersGroup = app.MapGroup("/users").WithTags("Users");

usersGroup.MapPost("/register", async ([FromBody] UserRegisterRequest request, IRepoUsuario repo) =>
{
    // Verificar si el email ya existe
    var existingUser = await repo.ObtenerPorEmailAsync(request.Email);
    if (existingUser != null)
        return Results.BadRequest(new { error = "El email ya está registrado" });

    var usuario = new Usuario
    {
        NombreUsuario = request.NombreUsuario,
        Email = request.Email,
        Contrasenia = request.Contrasenia, // En producción: hashear la contraseña
        Nacionalidad = new Nacionalidad { idNacionalidad = request.NacionalidadId }
    };
    
    await repo.InsertarAsync(usuario);
    return Results.Created($"/users/{usuario.idUsuario}", new { 
        id = usuario.idUsuario, 
        nombreUsuario = usuario.NombreUsuario,
        message = "Usuario registrado exitosamente" 
    });
})
.WithName("RegisterUser")
.WithOpenApi();

usersGroup.MapPost("/login", async ([FromBody] UserLoginRequest request, IRepoUsuario repo) =>
{
    var isValid = await repo.VerificarCredencialesAsync(request.Email, request.Contrasenia);
    if (!isValid)
        return Results.Unauthorized();
    
    // En producción: generar JWT token
    var user = await repo.ObtenerPorEmailAsync(request.Email);
    return Results.Ok(new { 
        message = "Login exitoso", 
        user = new { user.idUsuario, user.NombreUsuario, user.Email } 
    });
})
.WithName("LoginUser")
.WithOpenApi();

usersGroup.MapGet("/{id}", async (uint id, IRepoUsuario repo) =>
{
    var user = await repo.ObtenerPorIdAsync(id);
    return user is not null ? Results.Ok(user) : Results.NotFound(new { error = "Usuario no encontrado" });
})
.WithName("GetUserById")
.WithOpenApi();

// === ENDPOINTS DE PLAYLIST ===
var playlistsGroup = app.MapGroup("/playlists").WithTags("Playlists");

playlistsGroup.MapGet("/user/{userId}", async (uint userId, IRepoPlaylist repo) =>
{
    var playlists = await repo.ObtenerPorUsuarioAsync(userId);
    return Results.Ok(playlists);
})
.WithName("GetUserPlaylists")
.WithOpenApi();

playlistsGroup.MapGet("/{id}", async (uint id, IRepoPlaylist repo) =>
{
    var playlist = await repo.ObtenerConCancionesAsync(id);
    return playlist is not null ? Results.Ok(playlist) : Results.NotFound(new { error = "Playlist no encontrada" });
})
.WithName("GetPlaylistById")
.WithOpenApi();

playlistsGroup.MapPost("/", async ([FromBody] PlaylistCreateRequest request, IRepoPlaylist repo) =>
{
    var playlist = new PlayList
    {
        Nombre = request.Nombre,
        Usuario = new Usuario { idUsuario = request.UsuarioId }
    };
    
    await repo.InsertarAsync(playlist);
    return Results.Created($"/playlists/{playlist.idPlaylist}", new { 
        id = playlist.idPlaylist, 
        message = "Playlist creada exitosamente" 
    });
})
.WithName("CreatePlaylist")
.WithOpenApi();

playlistsGroup.MapPost("/{playlistId}/songs/{songId}", async (uint playlistId, uint songId, IRepoPlaylist repo) =>
{
    var success = await repo.AgregarCancionAsync(playlistId, songId);
    return success ? Results.Ok(new { message = "Canción agregada a la playlist" }) 
                   : Results.BadRequest(new { error = "Error al agregar canción" });
})
.WithName("AddSongToPlaylist")
.WithOpenApi();

playlistsGroup.MapDelete("/{playlistId}/songs/{songId}", async (uint playlistId, uint songId, IRepoPlaylist repo) =>
{
    var success = await repo.RemoverCancionAsync(playlistId, songId);
    return success ? Results.Ok(new { message = "Canción removida de la playlist" }) 
                   : Results.BadRequest(new { error = "Error al remover canción" });
})
.WithName("RemoveSongFromPlaylist")
.WithOpenApi();

// === ENDPOINTS DE GÉNERO ===
var genresGroup = app.MapGroup("/genres").WithTags("Genres");

genresGroup.MapGet("/", async (IRepoGenero repo) =>
{
    var genres = await repo.ObtenerTodosAsync();
    return Results.Ok(genres);
})
.WithName("GetAllGenres")
.WithOpenApi();

genresGroup.MapGet("/popular", async (IRepoGenero repo) =>
{
    var genres = await repo.ObtenerGenerosPopularesAsync();
    return Results.Ok(genres);
})
.WithName("GetPopularGenres")
.WithOpenApi();

// === ENDPOINTS DE REPRODUCCIÓN ===
var playbackGroup = app.MapGroup("/playback").WithTags("Playback");

playbackGroup.MapGet("/user/{userId}/history", async (uint userId, IRepoReproduccion repo, [FromQuery] int limit = 50) =>
{
    var history = await repo.ObtenerHistorialUsuarioAsync(userId, limit);
    return Results.Ok(history);
})
.WithName("GetUserPlaybackHistory")
.WithOpenApi();

playbackGroup.MapGet("/user/{userId}/top-songs", async (uint userId, IRepoReproduccion repo, [FromQuery] int limit = 10) =>
{
    var topSongs = await repo.ObtenerCancionesMasEscuchadasAsync(userId, limit);
    return Results.Ok(topSongs);
})
.WithName("GetUserTopSongs")
.WithOpenApi();

// === ENDPOINTS DE SUSCRIPCIÓN ===
var subscriptionGroup = app.MapGroup("/subscriptions").WithTags("Subscriptions");

subscriptionGroup.MapGet("/user/{userId}/active", async (uint userId, IRepoRegistro repo) =>
{
    var activeSubscription = await repo.ObtenerSuscripcionActivaAsync(userId);
    return activeSubscription is not null ? Results.Ok(activeSubscription) 
                                         : Results.NotFound(new { error = "No hay suscripción activa" });
})
.WithName("GetActiveSubscription")
.WithOpenApi();

subscriptionGroup.MapGet("/types", async (IRepoTipoSuscripcion repo) =>
{
    var types = await repo.ObtenerTodosAsync();
    return Results.Ok(types);
})
.WithName("GetSubscriptionTypes")
.WithOpenApi();

// === ENDPOINTS DE NACIONALIDAD ===
var countriesGroup = app.MapGroup("/countries").WithTags("Countries");

countriesGroup.MapGet("/", async (IRepoNacionalidad repo) =>
{
    var countries = await repo.ObtenerTodosAsync();
    return Results.Ok(countries);
})
.WithName("GetAllCountries")
.WithOpenApi();

// === ENDPOINT DE BÚSQUEDA GLOBAL ===
app.MapGet("/search", async (string q, IRepoAlbum albumRepo, IRepoArtista artistRepo, IRepoCancion songRepo) =>
{
    var albums = await albumRepo.BuscarTextoAsync(q);
    var artists = await artistRepo.BuscarTextoAsync(q);
    var songs = await songRepo.BuscarTextoAsync(q);
    
    return Results.Ok(new
    {
        query = q,
        albums,
        artists,
        songs
    });
})
.WithTags("Search")
.WithName("GlobalSearch")
.WithOpenApi();

// === INICIO DE LA APLICACIÓN ===
app.Run();

// === MODELOS DE REQUEST ===
public record AlbumCreateRequest(
    [Required] string Titulo,
    [Required] uint ArtistaId,
    string? Portada = null,
    DateTime? FechaLanzamiento = null);

public record AlbumUpdateRequest(
    string? Titulo = null,
    uint? ArtistaId = null,
    string? Portada = null,
    DateTime? FechaLanzamiento = null);

public record ArtistaCreateRequest(
    [Required] string NombreArtistico,
    string? Nombre = null,
    string? Apellido = null);

public record UserRegisterRequest(
    [Required] string NombreUsuario,
    [Required][EmailAddress] string Email,
    [Required] string Contrasenia,
    [Required] uint NacionalidadId);

public record UserLoginRequest(
    [Required] string Email,
    [Required] string Contrasenia);

public record PlaylistCreateRequest(
    [Required] string Nombre,
    [Required] uint UsuarioId);