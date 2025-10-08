using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.Mvc.Models;
using System.Diagnostics;

namespace Spotify.Controllers
{
    public class AlbumesController : Controller
    {
        private readonly IRepoAlbumAsync _repoAlbum;
        private readonly IRepoArtistaAsync _repoArtista;

        public AlbumesController(IRepoAlbumAsync repoAlbum, IRepoArtistaAsync repoArtista)
        {
            _repoAlbum = repoAlbum;
            _repoArtista = repoArtista;
        }

        // GET: Albumes
        public async Task<IActionResult> Index()
        {
            try
            {
                var albumes = await _repoAlbum.Obtener();
                var viewModel = albumes.Select(a => new AlbumViewModel
                {
                    IdAlbum = a.idAlbum,
                    Titulo = a.Titulo,
                    FechaLanzamiento = a.FechaLanzamiento,
                    NombreArtista = a.artista?.NombreArtistico ?? "Artista Desconocido",
                    IdArtista = a.artista?.idArtista ?? 0
                }).ToList();
            
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar álbumes: {ex.Message}";
                return View(new List<AlbumViewModel>());
            }
        }

        // GET: Albumes/Details/5
        public async Task<IActionResult> Details(uint id)
        {
            try
            {
                var album = await _repoAlbum.DetalleDeAsync(id);
                if (album == null)
                {
                    TempData["ErrorMessage"] = "Álbum no encontrado";
                    return RedirectToAction(nameof(Index));
                }
            
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar detalles: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Albumes/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var artistas = await _repoArtista.Obtener();
                var viewModel = new AlbumCreateViewModel
                {
                    Artistas = artistas.ToList()
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar formulario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Albumes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlbumCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Artistas = (await _repoArtista.Obtener()).ToList();
                return View(viewModel);
            }

            try
            {
                var artista = await _repoArtista.DetalleDeAsync(viewModel.ArtistaId);
                if (artista == null)
                {
                    ModelState.AddModelError("ArtistaId", "Artista no válido");
                    viewModel.Artistas = (await _repoArtista.Obtener()).ToList();
                    return View(viewModel);
                }

                var album = new Album
                {
                    Titulo = viewModel.Titulo,
                    FechaLanzamiento = DateTime.Now,
                    artista = artista
                };

                var resultado = await _repoAlbum.AltaAsync(album);
                TempData["SuccessMessage"] = $"Álbum '{resultado.Titulo}' creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear álbum: {ex.Message}");
                viewModel.Artistas = (await _repoArtista.Obtener()).ToList();
                return View(viewModel);
            }
        }

        // POST: Albumes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            try
            {
                await _repoAlbum.EliminarAsync(id);
                TempData["SuccessMessage"] = "Álbum eliminado correctamente";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar álbum: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}