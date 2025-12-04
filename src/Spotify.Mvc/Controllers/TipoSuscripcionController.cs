namespace Spotify.Mvc.Controllers;

[Authorize(Roles = "3")]
public class SubscriptionController : Controller
{
    private readonly IRepoTipoSuscripcion _repoTipoSuscripcion;
    private readonly IRepoRegistro _repoSuscripcion;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(
        IRepoTipoSuscripcion repoTipoSuscripcion,
        IRepoRegistro repoSuscripcion,
        ILogger<SubscriptionController> logger)
    {
        _repoTipoSuscripcion = repoTipoSuscripcion;
        _repoSuscripcion = repoSuscripcion;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var tiposSuscripcion = await _repoTipoSuscripcion.ObtenerTodosAsync();
            return View(tiposSuscripcion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipos de suscripción");
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
    public async Task<IActionResult> Create(TipoSuscripcionModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var tipoSuscripcion = new TipoSuscripcion
            {
                Tipo = model.Tipo,
                DuracionMeses = (uint)model.DuracionMeses,
                Costo = model.Costo,
                EstaActivo = true
            };

            await _repoTipoSuscripcion.InsertarAsync(tipoSuscripcion);
            
            TempData["SuccessMessage"] = "Tipo de suscripción creado exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear tipo de suscripción");
            TempData["ErrorMessage"] = "Error al crear tipo de suscripción";
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var tipoSuscripcion = await _repoTipoSuscripcion.ObtenerPorIdAsync(id);
            if (tipoSuscripcion == null)
                return NotFound();

            var model = new TipoSuscripcionModel
            {
                IdTipoSuscripcion = (int)tipoSuscripcion.IdTipoSuscripcion,
                Tipo = tipoSuscripcion.Tipo,
                DuracionMeses = (int)tipoSuscripcion.DuracionMeses,
                Costo = tipoSuscripcion.Costo
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cargar edición de tipo de suscripción {id}");
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TipoSuscripcionModel model)
    {
        if (id != model.IdTipoSuscripcion)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var tipoSuscripcion = await _repoTipoSuscripcion.ObtenerPorIdAsync(id);
            if (tipoSuscripcion == null)
                return NotFound();

            tipoSuscripcion.Tipo = model.Tipo;
            tipoSuscripcion.DuracionMeses = (uint)model.DuracionMeses;
            tipoSuscripcion.Costo = model.Costo;

            await _repoTipoSuscripcion.ActualizarAsync(tipoSuscripcion);
            
            TempData["SuccessMessage"] = "Tipo de suscripción actualizado exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar tipo de suscripción {id}");
            TempData["ErrorMessage"] = "Error al actualizar tipo de suscripción";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repoTipoSuscripcion.EliminarAsync(id);
            TempData["SuccessMessage"] = "Tipo de suscripción eliminado exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar tipo de suscripción {id}");
            TempData["ErrorMessage"] = "Error al eliminar tipo de suscripción";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var tipoSuscripcion = await _repoTipoSuscripcion.ObtenerPorIdAsync(id);
            if (tipoSuscripcion == null)
                return Json(new { success = false, message = "Tipo de suscripción no encontrado" });

            tipoSuscripcion.EstaActivo = !tipoSuscripcion.EstaActivo;
            await _repoTipoSuscripcion.ActualizarAsync(tipoSuscripcion);

            return Json(new { success = true, status = tipoSuscripcion.EstaActivo });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cambiar estado de tipo de suscripción {id}");
            return Json(new { success = false, message = "Error interno" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ActiveSubscriptions(int page = 1, int pageSize = 20)
    {
        try
        {
            var suscripciones = await _repoSuscripcion.ObtenerSuscripcionesActivasAsync(page, pageSize);
            var total = await _repoSuscripcion.ObtenerTotalSuscripcionesActivasAsync();

            var model = new PaginatedModel<Suscripcion>
            {
                Items = suscripciones,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = total
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener suscripciones activas");
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> UserSubscriptions(uint userId)
    {
        try
        {
            var suscripciones = await _repoSuscripcion.ObtenerSuscripcionesPorUsuarioAsync(userId);
            return View(suscripciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener suscripciones del usuario {userId}");
            return View("Error");
        }
    }
}