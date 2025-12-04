using Spotify.Core.Entidades;
using Spotify.Mvc.Models;

namespace Spotify.Mvc.Controllers;

public class SearchController : Controller
{
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoArtista _repoArtista;
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoGenero _repoGenero;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        IRepoAlbum repoAlbum,
        IRepoArtista repoArtista,
        IRepoCancion repoCancion,
        IRepoGenero repoGenero,
        ILogger<SearchController> logger)
    {
        _repoAlbum = repoAlbum;
        _repoArtista = repoArtista;
        _repoCancion = repoCancion;
        _repoGenero = repoGenero;
        _logger = logger;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Json(new { success = false, message = "Término de búsqueda vacío" });

        try
        {
            // Búsqueda básica para todos los usuarios
            var albumes = await _repoAlbum.BuscarTextoAsync(query);
            var artistas = await _repoArtista.BuscarTextoAsync(query);
            var canciones = await _repoCancion.BuscarTextoAsync(query);

            var result = new SearchResultModel
            {
                Query = query,
                Albumes = albumes.Take(5).ToList(),
                Artistas = artistas.Take(5).ToList(),
                Canciones = canciones.Take(10).ToList()
            };

            return Json(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error en búsqueda: {query}");
            return Json(new { success = false, message = "Error en búsqueda" });
        }
    }

    [HttpGet]
    [Authorize(Roles = "2,3")] // Solo usuarios registrados y admin
    public IActionResult Advanced()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "2,3")]
    public async Task<IActionResult> AdvancedSearch(AdvancedSearchModel model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

        try
        {
            // Búsqueda avanzada con filtros
            // Nota: Necesitarás implementar este método en tu repositorio
            var canciones = await _repoCancion.BuscarAvanzadoAsync(
                model.Query,
                model.GeneroId,
                model.ArtistaId,
                model.Anio,
                model.DuracionMin,
                model.DuracionMax);

            var result = new AdvancedSearchResultModel
            {
                Query = model.Query,
                Canciones = canciones,
                Total = canciones.Count()
            };

            return Json(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en búsqueda avanzada");
            return Json(new { success = false, message = "Error en búsqueda" });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ByGenre(int id)
    {
        try
        {
            var genero = await _repoGenero.ObtenerPorIdAsync(id);
            if (genero == null)
                return NotFound();

            var canciones = await _repoCancion.ObtenerPorGeneroAsync(id);

            var model = new SearchByGenreModel
            {
                Genero = genero,
                Canciones = canciones
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al buscar por género {id}");
            return View("Error");
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ByArtist(int id)
    {
        try
        {
            var artista = await _repoArtista.ObtenerPorIdAsync(id);
            if (artista == null)
                return NotFound();

            var canciones = await _repoCancion.ObtenerPorArtistaAsync(id);
            var albumes = await _repoAlbum.ObtenerPorArtistaAsync(id);

            var model = new SearchByArtistModel
            {
                Artista = artista,
                Canciones = canciones,
                Albumes = albumes
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al buscar por artista {id}");
            return View("Error");
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> QuickSearch(string term)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                return Json(new { success = true, results = new List<object>() });

            var albumes = await _repoAlbum.BuscarTextoAsync(term);
            var artistas = await _repoArtista.BuscarTextoAsync(term);
            var canciones = await _repoCancion.BuscarTextoAsync(term);

            var results = new List<object>();
            
            results.AddRange(artistas.Take(3).Select(a => new {
                type = "artista",
                id = a.IdArtista,
                name = a.NombreArtistico,
                image = a.FotoArtista
            }));
            
            results.AddRange(albumes.Take(3).Select(a => new {
                type = "album",
                id = a.IdAlbum,
                name = a.Titulo,
                image = a.Portada
            }));
            
            results.AddRange(canciones.Take(4).Select(c => new {
                type = "cancion",
                id = c.IdCancion,
                name = c.Titulo,
                artist = c.IdArtista
            }));

            return Json(new { success = true, results = results });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error en búsqueda rápida: {term}");
            return Json(new { success = false, message = "Error en búsqueda" });
        }
    }
}