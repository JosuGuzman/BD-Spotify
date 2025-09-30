// Controllers/ArtistasController.cs
using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.Mvc.Models;

namespace Spotify.Controllers;

public class ArtistasController : Controller
{
    private readonly IRepoArtista _repoArtista;

    public ArtistasController(IRepoArtista repoArtista)
    {
        _repoArtista = repoArtista;
    }

    public IActionResult Index()
    {
        var artistas = _repoArtista.Obtener();
        var viewModel = artistas.Select(a => new ArtistaViewModel
        {
            IdArtista = a.idArtista,
            NombreArtistico = a.NombreArtistico,
            Nombre = a.Nombre,
            Apellido = a.Apellido
        }).ToList();

        return View(viewModel);
    }

    public IActionResult Details(uint id)
    {
        var artista = _repoArtista.DetalleDe(id);
        if (artista == null)
            return NotFound();

        var viewModel = new ArtistaViewModel
        {
            IdArtista = artista.idArtista,
            NombreArtistico = artista.NombreArtistico,
            Nombre = artista.Nombre,
            Apellido = artista.Apellido
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        return View(new ArtistaCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ArtistaCreateViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var artista = new Artista
                {
                    NombreArtistico = viewModel.NombreArtistico,
                    Nombre = viewModel.Nombre,
                    Apellido = viewModel.Apellido
                };

                _repoArtista.Alta(artista);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear artista: {ex.Message}");
            }
        }
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(uint id)
    {
        try
        {
            _repoArtista.Eliminar(id);
            TempData["SuccessMessage"] = "Artista eliminado correctamente";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al eliminar artista: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}