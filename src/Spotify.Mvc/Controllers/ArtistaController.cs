// Controllers/ArtistasController.cs
using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.Mvc.Models;

namespace Spotify.Controllers;

public class ArtistasController : Controller
{
    private readonly IRepoArtistaAsync _repoArtista;

    public ArtistasController(IRepoArtistaAsync repoArtista)
    {
        _repoArtista = repoArtista;
    }

    // GET: Artistas
    public async Task<IActionResult> Index()
    {
        try
        {
            var artistas = await _repoArtista.Obtener();
            var viewModel = artistas.Select(a => new ArtistaViewModel
            {
                IdArtista = a.idArtista,
                NombreArtistico = a.NombreArtistico,
                Nombre = a.Nombre,
                Apellido = a.Apellido
            }).ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar artistas: {ex.Message}";
            return View(new List<ArtistaViewModel>());
        }
    }

    // GET: Artistas/Details/5
    public async Task<IActionResult> Details(uint id)
    {
        try
        {
            var artista = await _repoArtista.DetalleDeAsync(id);
            if (artista == null)
            {
                TempData["ErrorMessage"] = "Artista no encontrado";
                return RedirectToAction(nameof(Index));
            }
        
            var viewModel = new ArtistaViewModel
            {
                IdArtista = artista.idArtista,
                NombreArtistico = artista.NombreArtistico,
                Nombre = artista.Nombre,
                Apellido = artista.Apellido
            };
        
            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar detalles: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Artistas/Create
    public IActionResult Create()
    {
        return View(new ArtistaCreateViewModel());
    }

    // POST: Artistas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ArtistaCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        try
        {
            var artista = new Artista
            {
                NombreArtistico = viewModel.NombreArtistico,
                Nombre = viewModel.Nombre,
                Apellido = viewModel.Apellido
            };

            var resultado = await _repoArtista.AltaAsync(artista);
            TempData["SuccessMessage"] = $"Artista '{resultado.NombreArtistico}' creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al crear artista: {ex.Message}");
            return View(viewModel);
        }
    }

    // POST: Artistas/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(uint id)
    {
        try
        {
            await _repoArtista.EliminarAsync(id);
            TempData["SuccessMessage"] = "Artista eliminado correctamente";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al eliminar artista: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}