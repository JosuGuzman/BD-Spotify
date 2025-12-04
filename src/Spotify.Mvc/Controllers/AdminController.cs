using Spotify.Core.Entidades;
using Spotify.Mvc.Models;

namespace Spotify.Mvc.Controllers;

[Authorize(Roles = "3")]
public class AdminController : Controller
{
    private readonly IRepoUsuario _repoUsuario;
    private readonly IRepoArtista _repoArtista;
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoGenero _repoGenero;
    private readonly IRepoNacionalidad _repoNacionalidad;
    private readonly IRepoTipoSuscripcion _repoTipoSuscripcion;
    private readonly IRepoReproduccion _repoReproduccion;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IRepoUsuario repoUsuario,
        IRepoArtista repoArtista,
        IRepoAlbum repoAlbum,
        IRepoCancion repoCancion,
        IRepoGenero repoGenero,
        IRepoNacionalidad repoNacionalidad,
        IRepoTipoSuscripcion repoTipoSuscripcion,
        IRepoReproduccion repoReproduccion,
        ILogger<AdminController> logger)
    {
        _repoUsuario = repoUsuario;
        _repoArtista = repoArtista;
        _repoAlbum = repoAlbum;
        _repoCancion = repoCancion;
        _repoGenero = repoGenero;
        _repoNacionalidad = repoNacionalidad;
        _repoTipoSuscripcion = repoTipoSuscripcion;
        _repoReproduccion = repoReproduccion;
        _logger = logger;
    }

    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var model = new AdminDashboardModel
            {
                TotalUsuarios = await _repoUsuario.ObtenerTotalAsync(),
                TotalArtistas = await _repoArtista.ObtenerTotalAsync(),
                TotalAlbumes = await _repoAlbum.ObtenerTotalAsync(),
                TotalCanciones = await _repoCancion.ObtenerTotalAsync(),
                TotalGeneros = await _repoGenero.ObtenerTotalAsync(),
                TotalReproducciones = await _repoCancion.ObtenerTotalReproduccionesAsync(),
                NuevosUsuariosHoy = await _repoUsuario.ObtenerNuevosHoyAsync(),
                ReproduccionesHoy = await _repoReproduccion.ObtenerReproduccionesHoyAsync()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar dashboard admin");
            return View("Error");
        }
    }

    public async Task<IActionResult> Users(int page = 1, int pageSize = 20)
    {
        try
        {
            var usuarios = await _repoUsuario.ObtenerPaginadoAsync(page, pageSize);
            var total = await _repoUsuario.ObtenerTotalAsync();

            var model = new PaginatedModel<Usuario>
            {
                Items = usuarios,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = total
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleUserStatus(int id)
    {
        try
        {
            var usuario = await _repoUsuario.ObtenerPorIdAsync(id);
            if (usuario == null)
                return Json(new { success = false, message = "Usuario no encontrado" });

            usuario.EstaActivo = !usuario.EstaActivo;
            await _repoUsuario.ActualizarAsync(usuario);

            return Json(new { success = true, status = usuario.EstaActivo });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cambiar estado usuario {id}");
            return Json(new { success = false, message = "Error interno" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ChangeUserRole(int id, byte nuevoRol)
    {
        try
        {
            var usuario = await _repoUsuario.ObtenerPorIdAsync(id);
            if (usuario == null)
                return Json(new { success = false, message = "Usuario no encontrado" });

            usuario.IdRol = nuevoRol;
            await _repoUsuario.ActualizarAsync(usuario);

            return Json(new { success = true, nuevoRol = nuevoRol });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cambiar rol usuario {id}");
            return Json(new { success = false, message = "Error interno" });
        }
    }

    public async Task<IActionResult> Statistics()
    {
        try
        {
            var model = new StatisticsModel
            {
                TopCanciones = await _repoCancion.ObtenerTopCancionesAsync(10),
                TopArtistas = await _repoArtista.ObtenerTopArtistasAsync(10),
                TopGeneros = await _repoGenero.ObtenerTopGenerosAsync(10),
                ReproduccionesPorDia = await _repoReproduccion.ObtenerEstadisticasPorDiaAsync(30),
                NuevosUsuariosPorMes = await _repoUsuario.ObtenerNuevosPorMesAsync(12)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas");
            return View("Error");
        }
    }

    public async Task<IActionResult> SystemLogs()
    {
        try
        {
            // Obtener logs del sistema desde la tabla RegistroAuditoria
            // Esta funcionalidad dependerá de tu implementación
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener logs del sistema");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> GenerateReport(string reportType, DateTime? fechaInicio, DateTime? fechaFin)
    {
        try
        {
            var reportData = new ReportModel
            {
                Tipo = reportType,
                FechaInicio = fechaInicio ?? DateTime.Now.AddMonths(-1),
                FechaFin = fechaFin ?? DateTime.Now
            };

            // Según el tipo de reporte, obtener diferentes datos
            switch (reportType.ToLower())
            {
                case "usuarios":
                    reportData.Data = await _repoUsuario.ObtenerReporteUsuariosAsync(
                        reportData.FechaInicio, reportData.FechaFin);
                    break;
                case "reproducciones":
                    reportData.Data = await _repoReproduccion.ObtenerReporteReproduccionesAsync(
                        reportData.FechaInicio, reportData.FechaFin);
                    break;
                case "suscripciones":
                    // Implementar si tienes repo de suscripciones
                    break;
                default:
                    return Json(new { success = false, message = "Tipo de reporte no válido" });
            }

            return Json(new { success = true, data = reportData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al generar reporte {reportType}");
            return Json(new { success = false, message = "Error al generar reporte" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardData()
    {
        try
        {
            var data = new
            {
                totalUsuarios = await _repoUsuario.ObtenerTotalAsync(),
                totalArtistas = await _repoArtista.ObtenerTotalAsync(),
                totalAlbumes = await _repoAlbum.ObtenerTotalAsync(),
                totalCanciones = await _repoCancion.ObtenerTotalAsync(),
                totalReproducciones = await _repoCancion.ObtenerTotalReproduccionesAsync(),
                nuevosUsuariosHoy = await _repoUsuario.ObtenerNuevosHoyAsync(),
                reproduccionesHoy = await _repoReproduccion.ObtenerReproduccionesHoyAsync()
            };

            return Json(new { success = true, data = data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener datos del dashboard");
            return Json(new { success = false, message = "Error al obtener datos" });
        }
    }
}