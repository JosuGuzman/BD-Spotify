namespace Spotify.Mvc.Controllers;

public class ArtistController : Controller
{
    private readonly IRepoArtista _repoArtista;
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoNacionalidad _repoNacionalidad;
    private readonly ILogger<ArtistController> _logger;

    public ArtistController(
        IRepoArtista repoArtista,
        IRepoAlbum repoAlbum,
        IRepoCancion repoCancion,
        IRepoNacionalidad repoNacionalidad,
        ILogger<ArtistController> logger)
    {
        _repoArtista = repoArtista;
        _repoAlbum = repoAlbum;
        _repoCancion = repoCancion;
        _repoNacionalidad = repoNacionalidad;
        _logger = logger;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
    {
        try
        {
            var artistas = await _repoArtista.ObtenerPaginadoAsync(page, pageSize);
            var totalArtistas = await _repoArtista.ObtenerTotalAsync();
            
            var model = new PaginatedModel<Artista>
            {
                Items = artistas,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalArtistas
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lista de artistas");
            return View("Error");
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(uint id)
    {
        try
        {
            var artista = await _repoArtista.ObtenerPorIdAsync(id);
            if (artista == null)
                return NotFound();

            var albumes = await _repoAlbum.ObtenerPorArtistaAsync(id);
            var canciones = await _repoCancion.ObtenerPorArtistaAsync(id);

            var model = new ArtistDetailModel
            {
                Artista = artista,
                Albumes = albumes,
                Canciones = canciones
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener detalles del artista {id}");
            return View("Error");
        }
    }

    [Authorize(Roles = "3")]
    public async Task<IActionResult> Create()
    {
        var nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
        ViewBag.Nacionalidades = nacionalidades;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ArtistModel model)
    {
        if (!ModelState.IsValid)
        {
            var nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
            ViewBag.Nacionalidades = nacionalidades;
            return View(model);
        }

        try
        {
            var artista = new Artista
            {
                NombreArtistico = model.NombreArtistico,
                NombreReal = model.NombreReal,
                ApellidoReal = model.ApellidoReal,
                Biografia = model.Biografia,
                IdNacionalidad = model.IdNacionalidad,
                EstaActivo = true,
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };

            // Subir foto del artista
            if (model.FotoArtista != null)
            {
                var fileName = await GuardarArchivoAsync(model.FotoArtista, "artistas");
                artista.FotoArtista = fileName;
            }

            await _repoArtista.InsertarAsync(artista);
            
            TempData["SuccessMessage"] = "Artista creado exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear artista");
            TempData["ErrorMessage"] = "Error al crear artista";
            var nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
            ViewBag.Nacionalidades = nacionalidades;
            return View(model);
        }
    }

    [Authorize(Roles = "3")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var artista = await _repoArtista.ObtenerPorIdAsync(id);
            if (artista == null)
                return NotFound();

            var nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
            ViewBag.Nacionalidades = nacionalidades;

            var model = new ArtistModel
            {
                IdArtista = (int)artista.IdArtista,
                NombreArtistico = artista.NombreArtistico,
                NombreReal = artista.NombreReal,
                ApellidoReal = artista.ApellidoReal,
                Biografia = artista.Biografia,
                IdNacionalidad = artista.IdNacionalidad ?? 0
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cargar edición del artista {id}");
            return View("Error");
        }
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ArtistModel model)
    {
        if (id != model.IdArtista)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            var nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
            ViewBag.Nacionalidades = nacionalidades;
            return View(model);
        }

        try
        {
            var artista = await _repoArtista.ObtenerPorIdAsync(id);
            if (artista == null)
                return NotFound();

            artista.NombreArtistico = model.NombreArtistico;
            artista.NombreReal = model.NombreReal;
            artista.ApellidoReal = model.ApellidoReal;
            artista.Biografia = model.Biografia;
            artista.IdNacionalidad = model.IdNacionalidad;
            artista.FechaActualizacion = DateTime.Now;

            if (model.FotoArtista != null)
            {
                var fileName = await GuardarArchivoAsync(model.FotoArtista, "artistas");
                artista.FotoArtista = fileName;
            }

            await _repoArtista.ActualizarAsync(artista);
            
            TempData["SuccessMessage"] = "Artista actualizado exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar artista {id}");
            TempData["ErrorMessage"] = "Error al actualizar artista";
            var nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
            ViewBag.Nacionalidades = nacionalidades;
            return View(model);
        }
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var artista = await _repoArtista.ObtenerPorIdAsync(id);
            if (artista == null)
                return Json(new { success = false, message = "Artista no encontrado" });

            artista.EstaActivo = !artista.EstaActivo;
            artista.FechaActualizacion = DateTime.Now;
            await _repoArtista.ActualizarAsync(artista);

            return Json(new { success = true, status = artista.EstaActivo });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cambiar estado del artista {id}");
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