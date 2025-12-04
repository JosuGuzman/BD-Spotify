namespace Spotify.Mvc.Controllers;

[Authorize]
public class StatisticsController : Controller
{
    private readonly IRepoReproduccion _repoReproduccion;
    private readonly IRepoCancion _repoCancion;
    private readonly ILogger<StatisticsController> _logger;

    public StatisticsController(
        IRepoReproduccion repoReproduccion,
        IRepoCancion repoCancion,
        ILogger<StatisticsController> logger)
    {
        _repoReproduccion = repoReproduccion;
        _repoCancion = repoCancion;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> MyListeningHistory()
    {
        try
        {
            var userId = uint.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var historial = await _repoReproduccion.ObtenerHistorialUsuarioAsync(userId);
            
            return View(historial);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de escucha");
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> MyTopSongs(int days = 30)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var fechaDesde = DateTime.Now.AddDays(-days);
            
            var topCanciones = await _repoReproduccion.ObtenerTopCancionesUsuarioAsync(userId, fechaDesde, 20);
            
            return View(topCanciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener top canciones últimos {days} días");
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> MyTopArtists(int days = 30)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var fechaDesde = DateTime.Now.AddDays(-days);
            
            var topArtistas = await _repoReproduccion.ObtenerTopArtistasUsuarioAsync(userId, fechaDesde, 10);
            
            return View(topArtistas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener top artistas últimos {days} días");
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListeningTime()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Obtener tiempo de escucha por día de la última semana
            var tiempoPorDia = await _repoReproduccion.ObtenerTiempoEscuchaPorDiaAsync(userId, 7);
            
            // Obtener tiempo total del mes
            var tiempoTotalMes = await _repoReproduccion.ObtenerTiempoEscuchaTotalAsync(
                userId, 
                DateTime.Now.AddMonths(-1), 
                DateTime.Now);
            
            ViewBag.TiempoTotalMes = tiempoTotalMes;
            
            return View(tiempoPorDia);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tiempo de escucha");
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> DiscoverWeekly()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Obtener recomendaciones basadas en el historial
            var recomendaciones = await _repoCancion.ObtenerRecomendacionesAsync(userId, 20);
            
            return View(recomendaciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener recomendaciones");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ClearHistory()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _repoReproduccion.LimpiarHistorialUsuarioAsync(userId);
            
            TempData["SuccessMessage"] = "Historial eliminado correctamente";
            return RedirectToAction("MyListeningHistory");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al limpiar historial");
            TempData["ErrorMessage"] = "Error al limpiar historial";
            return RedirectToAction("MyListeningHistory");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetListeningStats(int days = 30)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var stats = new
            {
                topSongs = await _repoReproduccion.ObtenerTopCancionesUsuarioAsync(userId, DateTime.Now.AddDays(-days), 10),
                topArtists = await _repoReproduccion.ObtenerTopArtistasUsuarioAsync(userId, DateTime.Now.AddDays(-days), 5),
                totalTime = await _repoReproduccion.ObtenerTiempoEscuchaTotalAsync(userId, DateTime.Now.AddDays(-days), DateTime.Now),
            };

            return Json(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de escucha");
            return Json(new { success = false, message = "Error al obtener estadísticas" });
        }
    }
}