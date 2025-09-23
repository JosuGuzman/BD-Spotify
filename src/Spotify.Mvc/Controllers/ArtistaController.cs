using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;

namespace Spotify.Mvc.Controllers;

public class ArtistasController : Controller
{
    private readonly IRepoArtista _repo;

    public ArtistasController(IRepoArtista repo)
    {
        _repo = repo;
    }

    // GET: /Artistas
    public IActionResult Index()
    {
        var artistas = _repo.Obtener();
        return View(artistas);
    }

    // GET: /Artistas/Details/5
    public IActionResult Details(uint id)
    {
        var artista = _repo.DetalleDe(id);
        if (artista is null) return NotFound();
        return View(artista);
    }

    // GET: /Artistas/Create
    public IActionResult Create() => View();

    // POST: /Artistas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Artista artista)
    {
        if (ModelState.IsValid)
        {
            _repo.Alta(artista);
            return RedirectToAction(nameof(Index));
        }
        return View(artista);
    }
}