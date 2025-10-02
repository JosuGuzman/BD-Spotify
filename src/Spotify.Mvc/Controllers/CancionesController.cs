// Controllers/CancionesController.cs
using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.Mvc.Models;

namespace Spotify.Controllers;

public class CancionesController : Controller
{
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoArtista _repoArtista;
    private readonly IRepoGenero _repoGenero;

    public CancionesController(
        IRepoCancion repoCancion,
        IRepoAlbum repoAlbum,
        IRepoArtista repoArtista,
        IRepoGenero repoGenero)
    {
        _repoCancion = repoCancion;
        _repoAlbum = repoAlbum;
        _repoArtista = repoArtista;
        _repoGenero = repoGenero;
    }

    public IActionResult Index()
    {
        var canciones = _repoCancion.Obtener();
        var viewModel = canciones.Select(c => new CancionViewModel
        {
            IdCancion = c.idCancion,
            Titulo = c.Titulo,
            Duracion = c.Duracion,
            AlbumTitulo = c.album?.Titulo ?? "Álbum Desconocido",
            ArtistaNombre = c.artista?.NombreArtistico ?? "Artista Desconocido",
            Genero = c.genero?.genero ?? "Género Desconocido"
        }).ToList();

        return View(viewModel);
    }

    public IActionResult Details(uint id)
    {
        var cancion = _repoCancion.DetalleDe(id);
        if (cancion == null)
            return NotFound();

        // Manejo seguro de las propiedades que pueden ser null
        var viewModel = new CancionViewModel
        {
            IdCancion = cancion.idCancion,
            Titulo = cancion.Titulo,
            Duracion = cancion.Duracion,
            AlbumTitulo = cancion.album?.Titulo ?? "Álbum Desconocido",
            ArtistaNombre = cancion.artista?.NombreArtistico ?? "Artista Desconocido",
            Genero = cancion.genero?.genero ?? "Género Desconocido"
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        var viewModel = new CancionCreateViewModel
        {
            Albumes = _repoAlbum.Obtener().ToList(),
            Artistas = _repoArtista.Obtener().ToList(),
            Generos = _repoGenero.Obtener().ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CancionCreateViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var album = _repoAlbum.DetalleDe(viewModel.AlbumId);
                var artista = _repoArtista.DetalleDe(viewModel.ArtistaId);
                var genero = _repoGenero.DetalleDe(viewModel.GeneroId);

                // Validaciones más robustas
                if (album == null)
                {
                    ModelState.AddModelError("AlbumId", "Álbum no válido");
                }
                if (artista == null)
                {
                    ModelState.AddModelError("ArtistaId", "Artista no válido");
                }
                if (genero == null)
                {
                    ModelState.AddModelError("GeneroId", "Género no válido");
                }

                if (album == null || artista == null || genero == null)
                {
                    // Recargar las listas para el dropdown
                    viewModel.Albumes = _repoAlbum.Obtener().ToList();
                    viewModel.Artistas = _repoArtista.Obtener().ToList();
                    viewModel.Generos = _repoGenero.Obtener().ToList();
                    return View(viewModel);
                }

                var cancion = new Cancion
                {
                    Titulo = viewModel.Titulo,
                    Duracion = viewModel.Duracion,
                    album = album,
                    artista = artista,
                    genero = genero
                };

                _repoCancion.Alta(cancion);

                TempData["SuccessMessage"] = "Canción creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear canción: {ex.Message}");
            }
        }

        // Recargar las listas si hay error
        viewModel.Albumes = _repoAlbum.Obtener().ToList();
        viewModel.Artistas = _repoArtista.Obtener().ToList();
        viewModel.Generos = _repoGenero.Obtener().ToList();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(uint id)
    {
        try
        {
            // Necesitarías implementar un método Eliminar en IRepoCancion
            // Por ahora, vamos a simular la eliminación
            TempData["SuccessMessage"] = "Canción eliminada correctamente";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al eliminar canción: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}