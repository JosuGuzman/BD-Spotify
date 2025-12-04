namespace Spotify.Mvc.Controllers;

public class SongController : Controller
{
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoArtista _repoArtista;
    private readonly IRepoGenero _repoGenero;
    private readonly IRepoReproduccion _repoReproduccion;
    private readonly ILogger<SongController> _logger;

    public SongController(
        IRepoCancion repoCancion,
        IRepoAlbum repoAlbum,
        IRepoArtista repoArtista,
        IRepoGenero repoGenero,
        IRepoReproduccion repoReproduccion,
        ILogger<SongController> logger)
    {
        _repoCancion = repoCancion;
        _repoAlbum = repoAlbum;
        _repoArtista = repoArtista;
        _repoGenero = repoGenero;
        _repoReproduccion = repoReproduccion;
        _logger = logger;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, byte? generoId = null, uint? artistaId = null)
    {
        try
        {
            IEnumerable<Cancion> canciones;
            
            if (generoId.HasValue)
                canciones = await _repoCancion.ObtenerPorGeneroAsync(generoId.Value);
            else if (artistaId.HasValue)
                canciones = await _repoCancion.ObtenerPorArtistaAsync(artistaId.Value);
            else
                canciones = await _repoCancion.ObtenerPaginadoAsync(page, pageSize);

            var model = new SongListModel
            {
                Canciones = canciones,
                Generos = await _repoGenero.ObtenerTodosAsync(),
                Artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000),
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = await _repoCancion.ObtenerTotalAsync()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lista de canciones");
            return View("Error");
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var cancion = await _repoCancion.ObtenerPorIdAsync(id);
            if (cancion == null)
                return NotFound();

            var album = await _repoAlbum.ObtenerPorIdAsync(cancion.IdAlbum);
            var artista = await _repoArtista.ObtenerPorIdAsync(cancion.IdArtista);
            var genero = await _repoGenero.ObtenerPorIdAsync(cancion.IdGenero);

            var model = new SongModel
            {
                IdCancion = (int)cancion.IdCancion,
                Titulo = cancion.Titulo,
                DuracionSegundos = (int)cancion.DuracionSegundos,
                IdAlbum = (int)cancion.IdAlbum,
                IdArtista = (int)cancion.IdArtista,
                IdGenero = cancion.IdGenero,
                ArchivoMP3 = null,
                ArtistaNombre = artista?.NombreArtistico ?? "Desconocido",
                Genero = genero?.genero ?? "Desconocido"
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener detalles de canción {id}");
            return View("Error");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Play(uint id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Incrementar contador de reproducciones
            await _repoCancion.IncrementarReproduccionesAsync(id);
            
            // Registrar en historial
            await _repoReproduccion.RegistrarReproduccionAsync(new Reproduccion
            {
                IdUsuario = (uint)userId,
                IdCancion = id,
                FechaReproduccion = DateTime.Now,
                DuracionReproducida = 0
            });

            // Obtener URL del archivo MP3
            var cancion = await _repoCancion.ObtenerPorIdAsync(id);
            
            return Json(new { 
                success = true, 
                mp3Url = cancion?.ArchivoMP3,
                titulo = cancion?.Titulo,
                artista = cancion?.IdArtista
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al reproducir canción {id}");
            return Json(new { success = false, message = "Error al reproducir" });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ToggleLike(uint id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var meGusta = new MeGusta
            {
                IdUsuario = (uint)userId,
                IdCancion = id,
                FechaMeGusta = DateTime.Now
            };

            // Esto necesitaría un repositorio para MeGusta
            // Por ahora, solo devolvemos éxito simulado
            var result = true; // Simulación
            
            return Json(new { 
                success = true, 
                isLiked = result 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al marcar como me gusta {id}");
            return Json(new { success = false, message = "Error interno" });
        }
    }

    [Authorize(Roles = "3")]
    public async Task<IActionResult> Create()
    {
        var model = new SongModel
        {
            Albumes = await _repoAlbum.ObtenerPaginadoAsync(1, 1000),
            Artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000),
            Generos = await _repoGenero.ObtenerTodosAsync()
        };
        
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SongModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Albumes = await _repoAlbum.ObtenerPaginadoAsync(1, 1000);
            model.Artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000);
            model.Generos = await _repoGenero.ObtenerTodosAsync();
            return View(model);
        }

        try
        {
            var cancion = new Cancion
            {
                Titulo = model.Titulo,
                DuracionSegundos = (uint)model.DuracionSegundos,
                IdAlbum = (uint)model.IdAlbum,
                IdArtista = (uint)model.IdArtista,
                IdGenero = model.IdGenero,
                EstaActiva = true,
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };

            // Subir archivo MP3
            if (model.ArchivoMP3 != null)
            {
                var fileName = await GuardarArchivoMP3Async(model.ArchivoMP3);
                cancion.ArchivoMP3 = fileName;
            }
            else
            {
                TempData["ErrorMessage"] = "El archivo MP3 es requerido";
                model.Albumes = await _repoAlbum.ObtenerPaginadoAsync(1, 1000);
                model.Artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000);
                model.Generos = await _repoGenero.ObtenerTodosAsync();
                return View(model);
            }

            await _repoCancion.InsertarAsync(cancion);
            
            TempData["SuccessMessage"] = "Canción creada exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear canción");
            TempData["ErrorMessage"] = "Error al crear canción";
            
            model.Albumes = await _repoAlbum.ObtenerPaginadoAsync(1, 1000);
            model.Artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000);
            model.Generos = await _repoGenero.ObtenerTodosAsync();
            
            return View(model);
        }
    }

    [Authorize(Roles = "3")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var cancion = await _repoCancion.ObtenerPorIdAsync(id);
            if (cancion == null)
                return NotFound();

            var model = new SongModel
            {
                IdCancion = (int)cancion.IdCancion,
                Titulo = cancion.Titulo,
                DuracionSegundos = (int)cancion.DuracionSegundos,
                IdAlbum = (int)cancion.IdAlbum,
                IdArtista = (int)cancion.IdArtista,
                IdGenero = cancion.IdGenero
            };

            ViewBag.Albumes = await _repoAlbum.ObtenerPaginadoAsync(1, 1000);
            ViewBag.Artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000);
            ViewBag.Generos = await _repoGenero.ObtenerTodosAsync();

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cargar edición de canción {id}");
            return View("Error");
        }
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SongModel model)
    {
        if (id != model.IdCancion)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.Albumes = await _repoAlbum.ObtenerPaginadoAsync(1, 1000);
            ViewBag.Artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000);
            ViewBag.Generos = await _repoGenero.ObtenerTodosAsync();
            return View(model);
        }

        try
        {
            var cancion = await _repoCancion.ObtenerPorIdAsync(id);
            if (cancion == null)
                return NotFound();

            cancion.Titulo = model.Titulo;
            cancion.DuracionSegundos = (uint)model.DuracionSegundos;
            cancion.IdAlbum = (uint)model.IdAlbum;
            cancion.IdArtista = (uint)model.IdArtista;
            cancion.IdGenero = model.IdGenero;
            cancion.FechaActualizacion = DateTime.Now;

            if (model.ArchivoMP3 != null)
            {
                var fileName = await GuardarArchivoMP3Async(model.ArchivoMP3);
                cancion.ArchivoMP3 = fileName;
            }

            await _repoCancion.ActualizarAsync(cancion);
            
            TempData["SuccessMessage"] = "Canción actualizada exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar canción {id}");
            TempData["ErrorMessage"] = "Error al actualizar canción";
            
            ViewBag.Albumes = await _repoAlbum.ObtenerPaginadoAsync(1, 1000);
            ViewBag.Artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000);
            ViewBag.Generos = await _repoGenero.ObtenerTodosAsync();
            
            return View(model);
        }
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repoCancion.EliminarAsync(id);
            TempData["SuccessMessage"] = "Canción eliminada exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar canción {id}");
            TempData["ErrorMessage"] = "Error al eliminar canción";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var cancion = await _repoCancion.ObtenerPorIdAsync(id);
            if (cancion == null)
                return Json(new { success = false, message = "Canción no encontrada" });

            cancion.EstaActiva = !cancion.EstaActiva;
            cancion.FechaActualizacion = DateTime.Now;
            await _repoCancion.ActualizarAsync(cancion);

            return Json(new { success = true, status = cancion.EstaActiva });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cambiar estado de canción {id}");
            return Json(new { success = false, message = "Error interno" });
        }
    }

    private async Task<string> GuardarArchivoMP3Async(IFormFile archivo)
    {
        // Validar que sea un archivo MP3
        var extension = Path.GetExtension(archivo.FileName).ToLower();
        if (extension != ".mp3")
            throw new ArgumentException("El archivo debe ser MP3");

        var uploadsFolder = Path.Combine("wwwroot", "uploads", "canciones");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + ".mp3";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        return $"/uploads/canciones/{uniqueFileName}";
    }
}