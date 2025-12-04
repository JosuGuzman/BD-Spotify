namespace Spotify.Mvc.Controllers;

public class AlbumController : Controller
{
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoArtista _repoArtista;
    private readonly IRepoCancion _repoCancion;
    private readonly ILogger<AlbumController> _logger;

    public AlbumController(
        IRepoAlbum repoAlbum,
        IRepoArtista repoArtista,
        IRepoCancion repoCancion,
        ILogger<AlbumController> logger)
    {
        _repoAlbum = repoAlbum;
        _repoArtista = repoArtista;
        _repoCancion = repoCancion;
        _logger = logger;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
    {
        try
        {
            var albumes = await _repoAlbum.ObtenerPaginadoAsync(page, pageSize);
            var totalAlbumes = await _repoAlbum.ObtenerTotalAsync();
            
            var model = new PaginatedModel<Album>
            {
                Items = albumes,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalAlbumes
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lista de álbumes");
            return View("Error");
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(uint id)
    {
        try
        {
            var album = await _repoAlbum.ObtenerPorIdAsync(id);
            if (album == null)
                return NotFound();

            var canciones = await _repoCancion.ObtenerPorAlbumAsync( id);
            var artista = await _repoArtista.ObtenerPorIdAsync(album.IdArtista);

            var model = new AlbumDetailModel
            {
                Album = album,
                Canciones = canciones,
                Artista = artista
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener detalles del álbum {id}");
            return View("Error");
        }
    }

    [Authorize(Roles = "3")]
    public async Task<IActionResult> Create()
    {
        var artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000);
        ViewBag.Artistas = artistas;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlbumModel model)
    {
        if (!ModelState.IsValid)
        {
            var artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000);
            ViewBag.Artistas = artistas;
            return View(model);
        }

        try
        {
            var album = new Album
            {
                Titulo = model.Titulo,
                IdArtista = (uint)model.IdArtista,
                FechaLanzamiento = model.FechaLanzamiento,
                EstaActivo = true,
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };

            // Subir portada del álbum
            if (model.Portada != null)
            {
                var fileName = await GuardarArchivoAsync(model.Portada, "portadas");
                album.Portada = fileName;
            }

            await _repoAlbum.InsertarAsync(album);
            
            TempData["SuccessMessage"] = "Álbum creado exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear álbum");
            TempData["ErrorMessage"] = "Error al crear álbum";
            var artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000);
            ViewBag.Artistas = artistas;
            return View(model);
        }
    }

    [Authorize(Roles = "3")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var album = await _repoAlbum.ObtenerPorIdAsync(id);
            if (album == null)
                return NotFound();

            var artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000);
            ViewBag.Artistas = artistas;

            var model = new AlbumModel
            {
                IdAlbum = (int)album.IdAlbum,
                Titulo = album.Titulo,
                IdArtista = (int)album.IdArtista,
                FechaLanzamiento = album.FechaLanzamiento
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cargar edición del álbum {id}");
            return View("Error");
        }
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AlbumModel model)
    {
        if (id != model.IdAlbum)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            var artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000);
            ViewBag.Artistas = artistas;
            return View(model);
        }

        try
        {
            var album = await _repoAlbum.ObtenerPorIdAsync(id);
            if (album == null)
                return NotFound();

            album.Titulo = model.Titulo;
            album.IdArtista = (uint)model.IdArtista;
            album.FechaLanzamiento = model.FechaLanzamiento;
            album.FechaActualizacion = DateTime.Now;

            if (model.Portada != null)
            {
                var fileName = await GuardarArchivoAsync(model.Portada, "portadas");
                album.Portada = fileName;
            }

            await _repoAlbum.ActualizarAsync(album);
            
            TempData["SuccessMessage"] = "Álbum actualizado exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar álbum {id}");
            TempData["ErrorMessage"] = "Error al actualizar álbum";
            var artistas = await _repoArtista.ObtenerPaginadoAsync(1, 1000);
            ViewBag.Artistas = artistas;
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
            await _repoAlbum.EliminarAsync(id);
            TempData["SuccessMessage"] = "Álbum eliminado exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar álbum {id}");
            TempData["ErrorMessage"] = "Error al eliminar álbum";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var album = await _repoAlbum.ObtenerPorIdAsync(id);
            if (album == null)
                return Json(new { success = false, message = "Álbum no encontrado" });

            album.EstaActivo = !album.EstaActivo;
            album.FechaActualizacion = DateTime.Now;
            await _repoAlbum.ActualizarAsync(album);

            return Json(new { success = true, status = album.EstaActivo });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cambiar estado del álbum {id}");
            return Json(new { success = false, message = "Error interno" });
        }
    }

    private async Task<string> GuardarArchivoAsync(IFormFile archivo, string subcarpeta)
    {
        var uploadsFolder = Path.Combine("wwwroot", "uploads", subcarpeta);
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        return $"/uploads/{subcarpeta}/{uniqueFileName}";
    }
}