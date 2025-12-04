namespace Spotify.Mvc.Controllers;

[Authorize]
public class PlaylistController : Controller
{
    private readonly IRepoPlaylist _repoPlaylist;
    private readonly IRepoCancion _repoCancion;
    private readonly ILogger<PlaylistController> _logger;

    public PlaylistController(
        IRepoPlaylist repoPlaylist,
        IRepoCancion repoCancion,
        ILogger<PlaylistController> logger)
    {
        _repoPlaylist = repoPlaylist;
        _repoCancion = repoCancion;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = uint.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var playlists = await _repoPlaylist.ObtenerPorUsuarioAsync(userId);
            
            return View(playlists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener playlists del usuario");
            return View("Error");
        }
    }

    public async Task<IActionResult> Details(uint id)
    {
        try
        {
            var playlist = await _repoPlaylist.ObtenerConCancionesAsync(id);
            if (playlist == null)
                return NotFound();

            // Verificar que el usuario tiene acceso
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (playlist.IdUsuario != userId && !playlist.EsPublica)
                return Forbid();

            return View(playlist);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener detalles de playlist {id}");
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlaylistModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var playlist = new Playlist
            {
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                IdUsuario = (uint)userId,
                EsPublica = model.EsPublica,
                EsSistema = false,
                EstaActiva = true,
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };

            await _repoPlaylist.InsertarAsync(playlist);
            
            TempData["SuccessMessage"] = "Playlist creada exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear playlist");
            TempData["ErrorMessage"] = "Error al crear playlist";
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var playlist = await _repoPlaylist.ObtenerPorIdAsync(id);
            
            if (playlist == null)
                return NotFound();

            if (playlist.IdUsuario != userId)
                return Forbid();

            var model = new PlaylistModel
            {
                IdPlaylist = (int)playlist.IdPlaylist,
                Nombre = playlist.Nombre,
                Descripcion = playlist.Descripcion,
                EsPublica = playlist.EsPublica
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cargar edición de playlist {id}");
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PlaylistModel model)
    {
        if (id != model.IdPlaylist)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var playlist = await _repoPlaylist.ObtenerPorIdAsync(id);
            
            if (playlist == null)
                return NotFound();

            if (playlist.IdUsuario != userId)
                return Forbid();

            playlist.Nombre = model.Nombre;
            playlist.Descripcion = model.Descripcion;
            playlist.EsPublica = model.EsPublica;
            playlist.FechaActualizacion = DateTime.Now;

            await _repoPlaylist.ActualizarAsync(playlist);
            
            TempData["SuccessMessage"] = "Playlist actualizada exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar playlist {id}");
            TempData["ErrorMessage"] = "Error al actualizar playlist";
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddSong(uint playlistId, uint songId)
    {
        try
        {
            var userId = uint.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var playlist = await _repoPlaylist.ObtenerPorIdAsync(playlistId);
            
            if (playlist == null || playlist.IdUsuario != userId)
                return Json(new { success = false, message = "No autorizado" });

            var success = await _repoPlaylist.AgregarCancionAsync(playlistId, songId);
            
            if (success)
                return Json(new { success = true, message = "Canción agregada" });
            else
                return Json(new { success = false, message = "Error al agregar canción" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al agregar canción a playlist {playlistId}");
            return Json(new { success = false, message = "Error interno" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RemoveSong(uint playlistId, uint songId)
    {
        try
        {
            var userId = uint.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var playlist = await _repoPlaylist.ObtenerPorIdAsync(playlistId);
            
            if (playlist == null || playlist.IdUsuario != userId)
                return Json(new { success = false, message = "No autorizado" });

            var success = await _repoPlaylist.RemoverCancionAsync(playlistId, songId);
            
            if (success)
                return Json(new { success = true, message = "Canción removida" });
            else
                return Json(new { success = false, message = "Error al remover canción" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al remover canción de playlist {playlistId}");
            return Json(new { success = false, message = "Error interno" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ReorderSongs(int playlistId, List<int> songIds)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var playlist = await _repoPlaylist.ObtenerPorIdAsync(playlistId);
            
            if (playlist == null || playlist.IdUsuario != userId)
                return Json(new { success = false, message = "No autorizado" });

            // Implementar reordenación
            // Esta funcionalidad dependerá de tu implementación de repositorio
            var success = true; // Simulación
            
            if (success)
                return Json(new { success = true, message = "Orden actualizado" });
            else
                return Json(new { success = false, message = "Error al actualizar orden" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al reordenar canciones en playlist {playlistId}");
            return Json(new { success = false, message = "Error interno" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var playlist = await _repoPlaylist.ObtenerPorIdAsync(id);
            
            if (playlist == null || playlist.IdUsuario != userId)
                return Forbid();

            await _repoPlaylist.EliminarAsync(id);
            
            TempData["SuccessMessage"] = "Playlist eliminada";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar playlist {id}");
            TempData["ErrorMessage"] = "Error al eliminar playlist";
            return RedirectToAction("Index");
        }
    }
}