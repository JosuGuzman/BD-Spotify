// Controllers/CancionesController.cs
using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.Mvc.Models;

namespace Spotify.Controllers;

public class CancionesController : Controller
{
    private readonly IRepoCancionAsync _repoCancion;
    private readonly IRepoAlbumAsync _repoAlbum;
    private readonly IRepoArtistaAsync _repoArtista;
    private readonly IRepoGeneroAsync _repoGenero;

    public CancionesController(
        IRepoCancionAsync repoCancion,
        IRepoAlbumAsync repoAlbum,
        IRepoArtistaAsync repoArtista,
        IRepoGeneroAsync repoGenero)
    {
        _repoCancion = repoCancion;
        _repoAlbum = repoAlbum;
        _repoArtista = repoArtista;
        _repoGenero = repoGenero;
    }

    // GET: Canciones
    public async Task<IActionResult> Index()
    {
        try
        {
            var canciones = await _repoCancion.Obtener();
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
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar canciones: {ex.Message}";
            return View(new List<CancionViewModel>());
        }
    }

    // GET: Canciones/Details/5
    public async Task<IActionResult> Details(uint id)
    {
        try
        {
            var cancion = await _repoCancion.DetalleDeAsync(id);
            if (cancion == null)
            {
                TempData["ErrorMessage"] = "Canción no encontrada";
                return RedirectToAction(nameof(Index));
            }
        
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
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar detalles: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Canciones/Create
    public async Task<IActionResult> Create()
    {
        try
        {
            var viewModel = new CancionCreateViewModel
            {
                Albumes = (await _repoAlbum.Obtener()).ToList(),
                Artistas = (await _repoArtista.Obtener()).ToList(),
                Generos = (await _repoGenero.Obtener()).ToList()
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar formulario: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Canciones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CancionCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            try
            {
                viewModel.Albumes = (await _repoAlbum.Obtener()).ToList();
                viewModel.Artistas = (await _repoArtista.Obtener()).ToList();
                viewModel.Generos = (await _repoGenero.Obtener()).ToList();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al recargar datos: {ex.Message}");
            }
            return View(viewModel);
        }

        try
        {
            var album = await _repoAlbum.DetalleDeAsync(viewModel.AlbumId);
            var artista = await _repoArtista.DetalleDeAsync(viewModel.ArtistaId);
            var genero = await _repoGenero.DetalleDeAsync(viewModel.GeneroId);

            if (album == null || artista == null || genero == null)
            {
                if (album == null) ModelState.AddModelError("AlbumId", "Álbum no válido.");
                if (artista == null) ModelState.AddModelError("ArtistaId", "Artista no válido.");
                if (genero == null) ModelState.AddModelError("GeneroId", "Género no válido.");
                
                viewModel.Albumes = (await _repoAlbum.Obtener()).ToList();
                viewModel.Artistas = (await _repoArtista.Obtener()).ToList();
                viewModel.Generos = (await _repoGenero.Obtener()).ToList();
                return View(viewModel);
            }

            var duracionTimeSpan = TimeSpan.FromSeconds(viewModel.DuracionSegundos);
    
            var cancion = new Cancion
            {
                Titulo = viewModel.Titulo,
                Duracion = duracionTimeSpan,
                album = album,
                artista = artista,
                genero = genero
            };
    
            var resultado = await _repoCancion.AltaAsync(cancion);
            
            TempData["SuccessMessage"] = $"Canción '{resultado.Titulo}' creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al crear canción: {ex.Message}");
            
            try
            {
                viewModel.Albumes = (await _repoAlbum.Obtener()).ToList();
                viewModel.Artistas = (await _repoArtista.Obtener()).ToList();
                viewModel.Generos = (await _repoGenero.Obtener()).ToList();
            }
            catch (Exception reloadEx)
            {
                ModelState.AddModelError("", $"Error al recargar datos: {reloadEx.Message}");
            }
            
            return View(viewModel);
        }
    }

    // POST: Canciones/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(uint id)
    {
        try
        {
            await _repoCancion.EliminarAsync(id);
            TempData["SuccessMessage"] = "Canción eliminada correctamente";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al eliminar canción: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}