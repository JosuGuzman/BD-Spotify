using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.ReposDapper;
using Spotify.DTOs;
using Scalar.AspNetCore;
using System.Data;
using MySqlConnector;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
var connectionString = builder.Configuration.GetConnectionString("MySQL");
builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection(connectionString));

// Repositorios Síncronos (para compatibilidad)
builder.Services.AddScoped<IRepoArtista, RepoArtista>();
builder.Services.AddScoped<IRepoAlbum, RepoAlbum>();
builder.Services.AddScoped<IRepoUsuario, RepoUsuario>();
builder.Services.AddScoped<IRepoGenero, RepoGenero>();
builder.Services.AddScoped<IRepoCancion, RepoCancion>();
builder.Services.AddScoped<IRepoNacionalidad, RepoNacionalidad>();
builder.Services.AddScoped<IRepoPlaylist, RepoPlaylist>();
builder.Services.AddScoped<IRepoReproduccion, RepoReproduccion>();
builder.Services.AddScoped<IRepoTipoSuscripcion, RepoTipoSuscripcion>();
builder.Services.AddScoped<IRepoRegistro, RepoSuscripcion>();

// Repositorios Asíncronos (para la API)
builder.Services.AddScoped<IRepoArtistaAsync, RepoArtistaAsync>();
builder.Services.AddScoped<IRepoAlbumAsync, RepoAlbumAsync>();
builder.Services.AddScoped<IRepoUsuarioAsync, RepoUsuarioAsync>();
builder.Services.AddScoped<IRepoGeneroAsync, RepoGeneroAsync>();
builder.Services.AddScoped<IRepoCancionAsync, RepoCancionAsync>();
builder.Services.AddScoped<IRepoNacionalidadAsync, RepoNacionalidadAsync>();
builder.Services.AddScoped<IRepoPlaylistAsync, RepoPlaylistAsync>();
builder.Services.AddScoped<IRepoReproduccionAsync, RepoReproduccionAsync>();
builder.Services.AddScoped<IRepoTipoSuscripcionAsync, RepoTipoSuscripcionAsync>();

// FileService con IFormFile
var uploadPath = Path.Combine(builder.Environment.WebRootPath ?? "wwwroot", "uploads");
builder.Services.AddScoped(provider => new FileService(uploadPath));

builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Servir archivos estáticos desde wwwroot
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });
    app.MapScalarApiReference();
}

// ========== ARTISTAS ==========
app.MapGet("/api/artistas", async (IRepoArtistaAsync repo) =>
{
    try
    {
        var artistas = await repo.Obtener();
        var resultados = artistas.Select(a => new ArtistaOutputDTO
        {
            IdArtista = a.idArtista,
            NombreArtistico = a.NombreArtistico,
            Nombre = a.Nombre,
            Apellido = a.Apellido
        }).ToList();

        return Results.Ok(resultados);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener artistas: {ex.Message}");
    }
});

app.MapGet("/api/artistas/{id}", async (IRepoArtistaAsync repo, uint id) =>
{
    try
    {
        var artista = await repo.DetalleDeAsync(id);
        if (artista is null)
            return Results.NotFound($"Artista con ID {id} no encontrado");

        var resultado = new ArtistaOutputDTO
        {
            IdArtista = artista.idArtista,
            NombreArtistico = artista.NombreArtistico,
            Nombre = artista.Nombre,
            Apellido = artista.Apellido
        };

        return Results.Ok(resultado);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener artista: {ex.Message}");
    }
});

app.MapPost("/api/artistas", async (IRepoArtistaAsync repo, ArtistaInputDTO dto) =>
{
    try
    {
        var artista = new Artista
        {
            NombreArtistico = dto.NombreArtistico,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido
        };

        var resultado = await repo.AltaAsync(artista);
        
        return Results.Created($"/api/artistas/{resultado.idArtista}", new ArtistaOutputDTO
        {
            IdArtista = resultado.idArtista,
            NombreArtistico = resultado.NombreArtistico,
            Nombre = resultado.Nombre,
            Apellido = resultado.Apellido
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al crear artista: {ex.Message}");
    }
});

app.MapDelete("/api/artistas/{id}", async (IRepoArtistaAsync repo, uint id) =>
{
    try
    {
        await repo.EliminarAsync(id);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al eliminar artista: {ex.Message}");
    }
});

// ========== ÁLBUMES ==========
app.MapGet("/api/albumes", async (IRepoAlbumAsync repo, FileService fileService) =>
{
    try
    {
        var albumes = await repo.Obtener();
        var resultados = albumes.Select(a => new AlbumOutputDTO
        {
            IdAlbum = a.idAlbum,
            Titulo = a.Titulo,
            FechaLanzamiento = a.FechaLanzamiento,
            Artista = a.artista?.NombreArtistico ?? "Desconocido",
            IdArtista = a.artista?.idArtista ?? 0,
            Portada = fileService.ObtenerRutaArchivo(a.Portada, "portadas")
        }).ToList();

        return Results.Ok(resultados);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener álbumes: {ex.Message}");
    }
});

app.MapGet("/api/albumes/{id}", async (IRepoAlbumAsync repo, FileService fileService, uint id) =>
{
    try
    {
        var album = await repo.DetalleDeAsync(id);
        if (album is null)
            return Results.NotFound($"Álbum con ID {id} no encontrado");

        var resultado = new AlbumOutputDTO
        {
            IdAlbum = album.idAlbum,
            Titulo = album.Titulo,
            FechaLanzamiento = album.FechaLanzamiento,
            Artista = album.artista?.NombreArtistico ?? "Desconocido",
            IdArtista = album.artista?.idArtista ?? 0,
            Portada = fileService.ObtenerRutaArchivo(album.Portada, "portadas")
        };

        return Results.Ok(resultado);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener álbum: {ex.Message}");
    }
});

app.MapPost("/api/albumes", async (IRepoAlbumAsync repo, FileService fileService, HttpContext context) =>
{
    try
    {
        var form = await context.Request.ReadFormAsync();
        var archivoPortada = form.Files["Portada"];
        
        string nombrePortada = "default_album.png";
        if (archivoPortada != null && archivoPortada.Length > 0)
        {
            nombrePortada = await fileService.GuardarPortadaAsync(archivoPortada);
        }

        var album = new Album
        {
            Titulo = form["Titulo"].ToString(),
            artista = new Artista
            {
                idArtista = uint.Parse(form["IdArtista"].ToString()),
                NombreArtistico = string.Empty,
                Nombre = string.Empty,
                Apellido = string.Empty
            },
            Portada = nombrePortada
        };

        var resultado = await repo.AltaAsync(album);
        
        return Results.Created($"/api/albumes/{resultado.idAlbum}", new AlbumOutputDTO
        {
            IdAlbum = resultado.idAlbum,
            Titulo = resultado.Titulo,
            FechaLanzamiento = resultado.FechaLanzamiento,
            Artista = resultado.artista?.NombreArtistico ?? "Desconocido",
            IdArtista = resultado.artista?.idArtista ?? 0,
            Portada = fileService.ObtenerRutaArchivo(resultado.Portada, "portadas")
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al crear álbum: {ex.Message}");
    }
});

app.MapDelete("/api/albumes/{id}", async (IRepoAlbumAsync repo, FileService fileService, uint id) =>
{
    try
    {
        // Obtener el álbum primero para eliminar su portada
        var album = await repo.DetalleDeAsync(id);
        if (album != null && album.Portada != "default_album.png")
        {
            fileService.EliminarArchivo(album.Portada, "portadas");
        }

        await repo.EliminarAsync(id);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al eliminar álbum: {ex.Message}");
    }
});

// ========== CANCIONES ==========
app.MapGet("/api/canciones", async (IRepoCancionAsync repo, FileService fileService) =>
{
    try
    {
        var canciones = await repo.Obtener();
        var resultados = canciones.Select(c => new CancionOutputDTO
        {
            IdCancion = c.idCancion,
            Titulo = c.Titulo,
            Duracion = c.Duracion,
            Album = c.album?.Titulo ?? "Desconocido",
            Artista = c.artista?.NombreArtistico ?? "Desconocido",
            Genero = c.genero?.genero ?? "Desconocido",
            ArchivoMP3 = fileService.ObtenerRutaArchivo(c.ArchivoMP3, "canciones")
        }).ToList();

        return Results.Ok(resultados);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener canciones: {ex.Message}");
    }
});

app.MapGet("/api/canciones/{id}", async (IRepoCancionAsync repo, FileService fileService, uint id) =>
{
    try
    {
        var cancion = await repo.DetalleDeAsync(id);
        if (cancion is null)
            return Results.NotFound($"Canción con ID {id} no encontrada");

        var resultado = new CancionOutputDTO
        {
            IdCancion = cancion.idCancion,
            Titulo = cancion.Titulo,
            Duracion = cancion.Duracion,
            Album = cancion.album?.Titulo ?? "Desconocido",
            Artista = cancion.artista?.NombreArtistico ?? "Desconocido",
            Genero = cancion.genero?.genero ?? "Desconocido",
            ArchivoMP3 = fileService.ObtenerRutaArchivo(cancion.ArchivoMP3, "canciones")
        };

        return Results.Ok(resultado);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener canción: {ex.Message}");
    }
});

app.MapPost("/api/canciones", async (IRepoCancionAsync repo, FileService fileService, HttpContext context) =>
{
    try
    {
        var form = await context.Request.ReadFormAsync();
        
        // Validar archivo MP3
        var archivoMP3 = form.Files["ArchivoMP3"];
        if (archivoMP3 == null || archivoMP3.Length == 0)
            return Results.BadRequest("El archivo MP3 es requerido");

        string nombreMP3 = await fileService.GuardarCancionAsync(archivoMP3);

        var cancion = new Cancion
        {
            Titulo = form["Titulo"].ToString(),
            Duracion = TimeSpan.Parse(form["Duracion"].ToString()),
            album = new Album
            {
                idAlbum = uint.Parse(form["IdAlbum"].ToString()),
                Titulo = string.Empty,
                artista = new Artista
                {
                    idArtista = uint.Parse(form["IdArtista"].ToString()),
                    NombreArtistico = string.Empty,
                    Nombre = string.Empty,
                    Apellido = string.Empty
                }
            },
            artista = new Artista
            {
                idArtista = uint.Parse(form["IdArtista"].ToString()),
                NombreArtistico = string.Empty,
                Nombre = string.Empty,
                Apellido = string.Empty
            },
            genero = new Genero { idGenero = byte.Parse(form["IdGenero"].ToString()), genero = string.Empty },
            ArchivoMP3 = nombreMP3
        };

        var resultado = await repo.AltaAsync(cancion);
        
        return Results.Created($"/api/canciones/{resultado.idCancion}", new CancionOutputDTO
        {
            IdCancion = resultado.idCancion,
            Titulo = resultado.Titulo,
            Duracion = resultado.Duracion,
            Album = resultado.album?.Titulo ?? "Desconocido",
            Artista = resultado.artista?.NombreArtistico ?? "Desconocido",
            Genero = resultado.genero?.genero ?? "Desconocido",
            ArchivoMP3 = fileService.ObtenerRutaArchivo(resultado.ArchivoMP3, "canciones")
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al crear canción: {ex.Message}");
    }
});

app.MapDelete("/api/canciones/{id}", async (IRepoCancionAsync repo, FileService fileService, uint id) =>
{
    try
    {
        // Obtener la canción primero para eliminar su archivo MP3
        var cancion = await repo.DetalleDeAsync(id);
        if (cancion != null && !string.IsNullOrEmpty(cancion.ArchivoMP3))
        {
            fileService.EliminarArchivo(cancion.ArchivoMP3, "canciones");
        }

        await repo.EliminarAsync(id);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al eliminar canción: {ex.Message}");
    }
});

app.MapPost("/api/canciones/buscar", async (IRepoCancionAsync repo, BusquedaDTO dto) =>
{
    try
    {
        var resultados = await repo.Matcheo(dto.Termino);
        return Results.Ok(new { termino = dto.Termino, resultados });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error en búsqueda: {ex.Message}");
    }
});

// ========== GÉNEROS ==========
app.MapGet("/api/generos", async (IRepoGeneroAsync repo) =>
{
    try
    {
        var generos = await repo.Obtener();
        var resultados = generos.Select(g => new GeneroOutputDTO
        {
            IdGenero = g.idGenero,
            Genero = g.genero,
            Descripcion = g.Descripcion
        }).ToList();

        return Results.Ok(resultados);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener géneros: {ex.Message}");
    }
});

app.MapGet("/api/generos/{id}", async (IRepoGeneroAsync repo, byte id) =>
{
    try
    {
        var genero = await repo.DetalleDeAsync(id);
        if (genero is null)
            return Results.NotFound($"Género con ID {id} no encontrado");

        var resultado = new GeneroOutputDTO
        {
            IdGenero = genero.idGenero,
            Genero = genero.genero,
            Descripcion = genero.Descripcion
        };

        return Results.Ok(resultado);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener género: {ex.Message}");
    }
});

app.MapPost("/api/generos", async (IRepoGeneroAsync repo, GeneroInputDTO dto) =>
{
    try
    {
        var genero = new Genero
        {
            genero = dto.Genero,
            Descripcion = dto.Descripcion ?? string.Empty
        };

        var resultado = await repo.AltaAsync(genero);
        
        return Results.Created($"/api/generos/{resultado.idGenero}", new GeneroOutputDTO
        {
            IdGenero = resultado.idGenero,
            Genero = resultado.genero,
            Descripcion = resultado.Descripcion
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al crear género: {ex.Message}");
    }
});

app.MapDelete("/api/generos/{id}", async (IRepoGeneroAsync repo, byte id) =>
{
    try
    {
        await repo.EliminarAsync(id);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al eliminar género: {ex.Message}");
    }
});

// ========== USUARIOS ==========
app.MapGet("/api/usuarios", async (IRepoUsuarioAsync repo) =>
{
    try
    {
        var usuarios = await repo.Obtener();
        var resultados = usuarios.Select(u => new UsuarioOutputDTO
        {
            IdUsuario = u.idUsuario,
            NombreUsuario = u.NombreUsuario,
            Gmail = u.Gmail,
            Nacionalidad = u.nacionalidad?.Pais ?? "Desconocida"
        }).ToList();

        return Results.Ok(resultados);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener usuarios: {ex.Message}");
    }
});

app.MapGet("/api/usuarios/{id}", async (IRepoUsuarioAsync repo, uint id) =>
{
    try
    {
        var usuario = await repo.DetalleDeAsync(id);
        if (usuario is null)
            return Results.NotFound($"Usuario con ID {id} no encontrado");

        var resultado = new UsuarioOutputDTO
        {
            IdUsuario = usuario.idUsuario,
            NombreUsuario = usuario.NombreUsuario,
            Gmail = usuario.Gmail,
            Nacionalidad = usuario.nacionalidad?.Pais ?? "Desconocida"
        };

        return Results.Ok(resultado);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener usuario: {ex.Message}");
    }
});

app.MapPost("/api/usuarios", async (IRepoUsuarioAsync repo, UsuarioInputDTO dto) =>
{
    try
    {
        var usuario = new Usuario
        {
            NombreUsuario = dto.NombreUsuario,
            Gmail = dto.Gmail,
            Contrasenia = dto.Contrasenia,
            nacionalidad = new Nacionalidad { idNacionalidad = dto.Nacionalidad, Pais = string.Empty }
        };

        var resultado = await repo.AltaAsync(usuario);

        return Results.Created($"/api/usuarios/{resultado.idUsuario}", new UsuarioOutputDTO
        {
            IdUsuario = resultado.idUsuario,
            NombreUsuario = resultado.NombreUsuario,
            Gmail = resultado.Gmail,
            Nacionalidad = resultado.nacionalidad?.Pais ?? "Desconocida"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al crear usuario: {ex.Message}");
    }
});

// ========== NACIONALIDADES ==========
app.MapGet("/api/nacionalidades", async (IRepoNacionalidadAsync repo) =>
{
    try
    {
        var nacionalidades = await repo.Obtener();
        var resultados = nacionalidades.Select(n => new NacionalidadOutputDTO
        {
            IdNacionalidad = n.idNacionalidad,
            Pais = n.Pais
        }).ToList();

        return Results.Ok(resultados);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener nacionalidades: {ex.Message}");
    }
});

// ========== PLAYLISTS ==========
app.MapGet("/api/playlists", async (IRepoPlaylistAsync repo) =>
{
    try
    {
        var playlists = await repo.Obtener();
        var resultados = playlists.Select(p => new PlaylistOutputDTO
        {
            IdPlaylist = p.idPlaylist,
            Nombre = p.Nombre,
            Usuario = p.usuario?.NombreUsuario ?? "Desconocido"
        }).ToList();

        return Results.Ok(resultados);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener playlists: {ex.Message}");
    }
});

app.MapPost("/api/playlists", async (IRepoPlaylistAsync repo, PlaylistInputDTO dto) =>
{
    try
    {
        var playlist = new PlayList
        {
            Nombre = dto.Nombre,
            usuario = new Usuario
            {
                idUsuario = dto.IdUsuario,
                NombreUsuario = string.Empty,
                Gmail = string.Empty,
                Contrasenia = string.Empty,
                nacionalidad = new Nacionalidad { idNacionalidad = 0, Pais = string.Empty }
            },
            Canciones = new List<Cancion>()
        };

        var resultado = await repo.AltaAsync(playlist);

        return Results.Created($"/api/playlists/{resultado.idPlaylist}", new PlaylistOutputDTO
        {
            IdPlaylist = resultado.idPlaylist,
            Nombre = resultado.Nombre,
            Usuario = resultado.usuario?.NombreUsuario ?? "Desconocido"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al crear playlist: {ex.Message}");
    }
});

// ========== REPRODUCCIONES ==========
app.MapGet("/api/reproducciones", async (IRepoReproduccionAsync repo) =>
{
    try
    {
        var reproducciones = await repo.Obtener();
        var resultados = reproducciones.Select(r => new ReproduccionOutputDTO
        {
            IdHistorial = r.IdHistorial,
            Usuario = r.usuario?.NombreUsuario ?? "Desconocido",
            Cancion = r.cancion?.Titulo ?? "Desconocida",
            FechaReproduccion = r.FechaReproduccion
        }).ToList();

        return Results.Ok(resultados);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener reproducciones: {ex.Message}");
    }
});

app.MapPost("/api/reproducciones", async (IRepoReproduccionAsync repo, ReproduccionInputDTO dto) =>
{
    try
    {
        var reproduccion = new Reproduccion
        {
            usuario = new Usuario
            {
                idUsuario = dto.IdUsuario,
                NombreUsuario = string.Empty,
                Gmail = string.Empty,
                Contrasenia = string.Empty,
                nacionalidad = new Nacionalidad { idNacionalidad = 0, Pais = string.Empty }
            },
            cancion = new Cancion
            {
                idCancion = dto.IdCancion,
                Titulo = string.Empty,
                Duracion = TimeSpan.Zero,
                album = new Album
                {
                    idAlbum = 0u,
                    Titulo = string.Empty,
                    artista = new Artista
                    {
                        idArtista = 0u,
                        NombreArtistico = string.Empty,
                        Nombre = string.Empty,
                        Apellido = string.Empty
                    }
                },
                artista = new Artista
                {
                    idArtista = 0u,
                    NombreArtistico = string.Empty,
                    Nombre = string.Empty,
                    Apellido = string.Empty
                },
                genero = new Genero
                {
                    idGenero = (byte)0,
                    genero = string.Empty,
                    Descripcion = string.Empty
                },
                ArchivoMP3 = string.Empty
            },
            FechaReproduccion = dto.FechaReproduccion
        };

        var resultado = await repo.AltaAsync(reproduccion);

        return Results.Created($"/api/reproducciones/{resultado.IdHistorial}", new ReproduccionOutputDTO
        {
            IdHistorial = resultado.IdHistorial,
            Usuario = resultado.usuario?.NombreUsuario ?? "Desconocido",
            Cancion = resultado.cancion?.Titulo ?? "Desconocida",
            FechaReproduccion = resultado.FechaReproduccion
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al crear reproducción: {ex.Message}");
    }
});

// ========== TIPOS DE SUSCRIPCIÓN ==========
app.MapGet("/api/tipos-suscripcion", async (IRepoTipoSuscripcionAsync repo) =>
{
    try
    {
        var tipos = await repo.Obtener();
        var resultados = tipos.Select(t => new TipoSuscripcionOutputDTO
        {
            IdTipoSuscripcion = t.IdTipoSuscripcion,
            Tipo = t.Tipo,
            Duracion = t.Duracion,
            Costo = t.Costo
        }).ToList();

        return Results.Ok(resultados);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener tipos de suscripción: {ex.Message}");
    }
});

// ========== ENDPOINTS ADICIONALES ==========
app.MapGet("/api/estadisticas", async (
    IRepoArtistaAsync repoArtista,
    IRepoAlbumAsync repoAlbum,
    IRepoCancionAsync repoCancion,
    IRepoUsuarioAsync repoUsuario,
    IRepoGeneroAsync repoGenero) =>
{
    try
    {
        var artistas = await repoArtista.Obtener();
        var albumes = await repoAlbum.Obtener();
        var canciones = await repoCancion.Obtener();
        var usuarios = await repoUsuario.Obtener();
        var generos = await repoGenero.Obtener();

        var estadisticas = new
        {
            TotalArtistas = artistas.Count,
            TotalAlbumes = albumes.Count,
            TotalCanciones = canciones.Count,
            TotalUsuarios = usuarios.Count,
            TotalGeneros = generos.Count,
            ArtistasRecientes = artistas.Take(5).Select(a => a.NombreArtistico),
            AlbumesRecientes = albumes.Take(5).Select(a => a.Titulo)
        };

        return Results.Ok(estadisticas);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al obtener estadísticas: {ex.Message}");
    }
});

app.Run();