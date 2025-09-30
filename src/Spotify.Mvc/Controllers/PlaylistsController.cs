// Controllers/PlaylistsController.cs
using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.Mvc.Models;

namespace Spotify.Controllers;

public class PlaylistsController : Controller
{
    private readonly IRepoPlaylist _repoPlaylist;
    private readonly IRepoUsuario _repoUsuario;
    private readonly IRepoCancion _repoCancion;

    public PlaylistsController(
        IRepoPlaylist repoPlaylist,
        IRepoUsuario repoUsuario,
        IRepoCancion repoCancion)
    {
        _repoPlaylist = repoPlaylist;
        _repoUsuario = repoUsuario;
        _repoCancion = repoCancion;
    }

    public IActionResult Index()
    {
        var playlists = _repoPlaylist.Obtener();
        var viewModel = playlists.Select(p => new PlaylistViewModel
        {
            IdPlaylist = p.idPlaylist,
            Nombre = p.Nombre,
            UsuarioNombre = p.usuario.NombreUsuario,
            CantidadCanciones = p.Canciones?.Count ?? 0
        }).ToList();

        return View(viewModel);
    }

    public IActionResult Details(uint id)
    {
        var canciones = _repoPlaylist.DetallePlaylist(id);
        var playlist = _repoPlaylist.DetalleDe(id);

        if (playlist == null)
            return NotFound();

        var viewModel = new PlaylistDetailViewModel
        {
            IdPlaylist = playlist.idPlaylist,
            Nombre = playlist.Nombre,
            UsuarioNombre = playlist.usuario.NombreUsuario,
            Canciones = canciones?.Select(c => new CancionViewModel
            {
                IdCancion = c.idCancion,
                Titulo = c.Titulo,
                Duracion = c.Duracion,
                AlbumTitulo = c.album.Titulo,
                ArtistaNombre = c.artista.NombreArtistico,
                Genero = c.genero.genero
            }).ToList() ?? new List<CancionViewModel>(),
            TodasCanciones = _repoCancion.Obtener().ToList()
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        var viewModel = new PlaylistCreateViewModel
        {
            Usuarios = _repoUsuario.Obtener().ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PlaylistCreateViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var usuario = _repoUsuario.DetalleDe(viewModel.UsuarioId);
                if (usuario == null)
                {
                    ModelState.AddModelError("UsuarioId", "Usuario no válido");
                    viewModel.Usuarios = _repoUsuario.Obtener().ToList();
                    return View(viewModel);
                }

                var playlist = new PlayList
                {
                    Nombre = viewModel.Nombre,
                    usuario = usuario,
                    Canciones = new List<Cancion>()
                };

                _repoPlaylist.Alta(playlist);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear playlist: {ex.Message}");
            }
        }

        viewModel.Usuarios = _repoUsuario.Obtener().ToList();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AgregarCancion(uint playlistId, uint cancionId)
    {
        try
        {
            _repoPlaylist.AgregarCancion(playlistId, cancionId);
            TempData["SuccessMessage"] = "Canción agregada a la playlist";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al agregar canción: {ex.Message}";
        }
        return RedirectToAction("Details", new { id = playlistId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(uint id)
    {
        try
        {
            _repoPlaylist.Eliminar(id);
            TempData["SuccessMessage"] = "Playlist eliminada correctamente";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al eliminar playlist: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}