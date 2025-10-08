using Microsoft.AspNetCore.Mvc;
using Spotify.Core.Persistencia;
using Spotify.Mvc.Models;
using System.Diagnostics;

namespace Spotify.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoArtista _repoArtista;
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoUsuario _repoUsuario;
    private readonly IRepoGenero _repoGenero;

    public HomeController(
        ILogger<HomeController> logger,
        IRepoCancion repoCancion,
        IRepoArtista repoArtista,
        IRepoAlbum repoAlbum,
        IRepoUsuario repoUsuario,
        IRepoGenero repoGenero)
    {
        _logger = logger;
        _repoCancion = repoCancion;
        _repoArtista = repoArtista;
        _repoAlbum = repoAlbum;
        _repoUsuario = repoUsuario;
        _repoGenero = repoGenero;
    }

    public IActionResult Index()
    {
        try
        {
            var dashboardStats = new DashboardViewModel
            {
                TotalArtistas = _repoArtista.Obtener().Count,
                TotalAlbumes = _repoAlbum.Obtener().Count,
                TotalCanciones = _repoCancion.Obtener().Count,
                TotalUsuarios = _repoUsuario.Obtener().Count,
                TotalGeneros = _repoGenero.Obtener().Count,
            };

            return View(dashboardStats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el dashboard");
            var emptyStats = new DashboardViewModel();
            return View(emptyStats);
        }
    }

    [HttpPost]
    public IActionResult Buscar(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            TempData["ErrorMessage"] = "Por favor, ingresa un término de búsqueda";
            return RedirectToAction("Index");
        }

        try
        {
            var resultados = _repoCancion.Matcheo(query);
            ViewBag.Query = query;
            ViewBag.ResultCount = resultados?.Count ?? 0;
            
            return View("ResultadosBusqueda", resultados ?? new List<string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar la búsqueda para: {Query}", query);
            TempData["ErrorMessage"] = "Error al realizar la búsqueda. Por favor, intenta nuevamente.";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Estadisticas()
    {
        try
        {
            var stats = new EstadisticasViewModel
            {
                TotalArtistas = _repoArtista.Obtener().Count,
                TotalAlbumes = _repoAlbum.Obtener().Count,
                TotalCanciones = _repoCancion.Obtener().Count,
                TotalUsuarios = _repoUsuario.Obtener().Count,
                TotalGeneros = _repoGenero.Obtener().Count,
                ArtistasRecientes = _repoArtista.Obtener().Take(5).ToList(),
                AlbumesRecientes = _repoAlbum.Obtener().Take(5).ToList()
            };

            return View(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar las estadísticas");
            TempData["ErrorMessage"] = "Error al cargar las estadísticas";
            return RedirectToAction("Index");
        }
    }

    public IActionResult AcercaDe()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}