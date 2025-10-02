// Controllers/AlbumesController.cs
using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.Mvc.Models;

namespace Spotify.Controllers;

public class AlbumesController : Controller
{
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoArtista _repoArtista;

    public AlbumesController(IRepoAlbum repoAlbum, IRepoArtista repoArtista)
    {
        _repoAlbum = repoAlbum;
        _repoArtista = repoArtista;
    }

    public IActionResult Index()
    {
        var albumes = _repoAlbum.Obtener();
        var viewModel = albumes.Select(a => new AlbumViewModel
        {
            IdAlbum = a.idAlbum,
            Titulo = a.Titulo,
            FechaLanzamiento = a.FechaLanzamiento,
            NombreArtista = a.artista?.NombreArtistico ?? "Artista Desconocido", // Manejo de nulos
            IdArtista = a.artista?.idArtista ?? 0
        }).ToList();
    
        return View(viewModel);
    }

    public IActionResult Details(uint id)
    {
        var album = _repoAlbum.DetalleDe(id);
        if (album == null)
            return NotFound();
    
        var viewModel = new AlbumViewModel
        {
            IdAlbum = album.idAlbum,
            Titulo = album.Titulo,
            FechaLanzamiento = album.FechaLanzamiento,
            NombreArtista = album.artista?.NombreArtistico ?? "Artista Desconocido",
            IdArtista = album.artista?.idArtista ?? 0
        };
    
        return View(viewModel);
    }

    public IActionResult Create()
    {
        var artistas = _repoArtista.Obtener();
        var viewModel = new AlbumCreateViewModel
        {
            Artistas = artistas.ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(AlbumCreateViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var artista = _repoArtista.DetalleDe(viewModel.ArtistaId);
                if (artista == null)
                {
                    ModelState.AddModelError("ArtistaId", "Artista no válido");
                    viewModel.Artistas = _repoArtista.Obtener().ToList();
                    return View(viewModel);
                }

                var album = new Album
                {
                    Titulo = viewModel.Titulo,
                    FechaLanzamiento = DateTime.Now,
                    artista = artista
                };

                _repoAlbum.Alta(album);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear álbum: {ex.Message}");
            }
        }

        viewModel.Artistas = _repoArtista.Obtener().ToList();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(uint id)
    {
        try
        {
            _repoAlbum.Eliminar(id);
            TempData["SuccessMessage"] = "Álbum eliminado correctamente";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al eliminar álbum: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}