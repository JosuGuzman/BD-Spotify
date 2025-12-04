namespace Spotify.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoArtista _repoArtista;
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoGenero _repoGenero;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IRepoAlbum repoAlbum,
        IRepoArtista repoArtista,
        IRepoCancion repoCancion,
        IRepoGenero repoGenero,
        ILogger<HomeController> logger)
    {
        _repoAlbum = repoAlbum;
        _repoArtista = repoArtista;
        _repoCancion = repoCancion;
        _repoGenero = repoGenero;
        _logger = logger;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        try
        {
            var hora = DateTime.Now.Hour;
            ViewBag.Saludo = hora < 12 ? "¡Buenos días!" :
                            hora < 19 ? "¡Buenas tardes!" : "¡Buenas noches!";

            // Para usuarios autenticados, mostrar contenido personalizado
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                ViewBag.UserId = userId;
            }

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la página de inicio");
            return View("Error");
        }
    }

    [AllowAnonymous]
    public IActionResult About()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> GetHomeContent()
    {
        try
        {
            var content = new
            {
                AlbumesRecientes = await _repoAlbum.ObtenerAlbumesRecientesAsync(6),
                ArtistasPopulares = await _repoArtista.ObtenerArtistasPopularesAsync(8),
                CancionesPopulares = await _repoCancion.ObtenerCancionesPopularesAsync(10),
                Generos = await _repoGenero.ObtenerGenerosPopularesAsync()
            };

            return Json(new { success = true, data = content });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener contenido de inicio");
            return Json(new { success = false, message = "Error al cargar contenido" });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}