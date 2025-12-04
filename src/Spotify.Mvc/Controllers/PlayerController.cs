namespace Spotify.Mvc.Controllers;

[Authorize]
public class PlayerController : Controller
{
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoReproduccion _repoReproduccion;
    private readonly ILogger<PlayerController> _logger;

    public PlayerController(
        IRepoCancion repoCancion,
        IRepoReproduccion repoReproduccion,
        ILogger<PlayerController> logger)
    {
        _repoCancion = repoCancion;
        _repoReproduccion = repoReproduccion;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Play(uint songId)
    {
        try
        {
            var userId = uint.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Registrar reproducción
            await _repoCancion.IncrementarReproduccionesAsync(songId);
            await _repoReproduccion.RegistrarReproduccionAsync(new Reproduccion
            {
                IdUsuario = (uint)userId,
                IdCancion = (uint)songId,
                FechaReproduccion = DateTime.Now,
                DuracionReproducida = 0
            });

            var cancion = await _repoCancion.ObtenerPorIdAsync(songId);
            
            if (cancion == null)
                return Json(new { success = false, message = "Canción no encontrada" });

            return Json(new { 
                success = true, 
                song = new {
                    id = cancion.IdCancion,
                    title = cancion.Titulo,
                    artist = cancion.IdArtista, // Necesitarías obtener nombre del artista
                    album = cancion.IdAlbum,    // Necesitarías obtener nombre del álbum
                    duration = cancion.DuracionSegundos,
                    mp3Url = cancion.ArchivoMP3
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al reproducir canción {songId}");
            return Json(new { success = false, message = "Error al reproducir" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RecordProgress(int songId, int progressSeconds)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Actualizar progreso en el historial
            // Esta funcionalidad dependerá de tu implementación de repositorio
            var success = true; // Simulación
            
            if (success)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = "Error al actualizar progreso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al registrar progreso canción {songId}");
            return Json(new { success = false });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddToQueue(int songId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Agregar canción a la cola
            // Esta funcionalidad dependerá de tu implementación
            var success = true; // Simulación
            
            if (success)
                return Json(new { success = true, message = "Canción agregada a la cola" });
            else
                return Json(new { success = false, message = "Error al agregar canción" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al agregar canción {songId} a la cola");
            return Json(new { success = false, message = "Error interno" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleShuffle(bool enable)
    {
        try
        {
            // Implementar lógica de shuffle
            // Esto podría almacenarse en sesión o en la base de datos
            HttpContext.Session.SetString("PlayerShuffle", enable.ToString());
            
            return Json(new { success = true, shuffle = enable });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cambiar modo shuffle a {enable}");
            return Json(new { success = false });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleRepeat(string mode)
    {
        try
        {
            // Modos: "none", "one", "all"
            HttpContext.Session.SetString("PlayerRepeat", mode);
            
            return Json(new { success = true, repeat = mode });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cambiar modo repeat a {mode}");
            return Json(new { success = false });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Current()
    {
        try
        {
            // Obtener canción actualmente en reproducción
            var currentSongId = HttpContext.Session.GetString("CurrentSongId");
            
            if (string.IsNullOrEmpty(currentSongId) || !int.TryParse(currentSongId, out int songId))
                return Json(new { success = false, message = "No hay canción en reproducción" });

            var cancion = await _repoCancion.ObtenerPorIdAsync(songId);
            
            if (cancion == null)
                return Json(new { success = false, message = "Canción no encontrada" });

            return Json(new { 
                success = true, 
                song = new {
                    id = cancion.IdCancion,
                    title = cancion.Titulo,
                    duration = cancion.DuracionSegundos,
                    mp3Url = cancion.ArchivoMP3
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener canción actual");
            return Json(new { success = false, message = "Error interno" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSongInfo(int id)
    {
        try
        {
            var cancion = await _repoCancion.ObtenerPorIdAsync(id);
            
            if (cancion == null)
                return Json(new { success = false, message = "Canción no encontrada" });

            return Json(new { 
                success = true, 
                song = new {
                    id = cancion.IdCancion,
                    title = cancion.Titulo,
                    duration = cancion.DuracionSegundos,
                    mp3Url = cancion.ArchivoMP3,
                    albumId = cancion.IdAlbum,
                    artistId = cancion.IdArtista
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener información de canción {id}");
            return Json(new { success = false, message = "Error interno" });
        }
    }
}