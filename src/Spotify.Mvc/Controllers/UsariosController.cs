// Controllers/UsuariosController.cs
using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.Mvc.Models;

namespace Spotify.Controllers;

public class UsuariosController : Controller
{
    private readonly IRepoUsuario _repoUsuario;
    private readonly IRepoNacionalidad _repoNacionalidad;

    public UsuariosController(IRepoUsuario repoUsuario, IRepoNacionalidad repoNacionalidad)
    {
        _repoUsuario = repoUsuario;
        _repoNacionalidad = repoNacionalidad;
    }

    public IActionResult Index()
    {
        var usuarios = _repoUsuario.Obtener();
        var viewModel = new List<UsuarioViewModel>();
    
        foreach (var usuario in usuarios)
        {
            // Manejo seguro de la nacionalidad
            string pais = "Desconocida";
            if (usuario.nacionalidad != null && usuario.nacionalidad.idNacionalidad > 0)
            {
                var nacionalidad = _repoNacionalidad.DetalleDe(usuario.nacionalidad.idNacionalidad);
                pais = nacionalidad?.Pais ?? "Desconocida";
            }
    
            viewModel.Add(new UsuarioViewModel
            {
                IdUsuario = usuario.idUsuario,
                NombreUsuario = usuario.NombreUsuario,
                Gmail = usuario.Gmail,
                Pais = pais
            });
        }
    
        return View(viewModel);
    }

    public IActionResult Details(uint id)
    {
        var usuario = _repoUsuario.DetalleDe(id);
        if (usuario == null)
            return NotFound();

        // Manejo seguro de la nacionalidad
        string pais = "Desconocida";
        if (usuario.nacionalidad != null && usuario.nacionalidad.idNacionalidad > 0)
        {
            var nacionalidad = _repoNacionalidad.DetalleDe(usuario.nacionalidad.idNacionalidad);
            pais = nacionalidad?.Pais ?? "Desconocida";
        }

        var viewModel = new UsuarioViewModel
        {
            IdUsuario = usuario.idUsuario,
            NombreUsuario = usuario.NombreUsuario,
            Gmail = usuario.Gmail,
            Pais = pais
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        var nacionalidades = _repoNacionalidad.Obtener();
        var viewModel = new UsuarioCreateViewModel
        {
            Nacionalidades = nacionalidades.ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UsuarioCreateViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var nacionalidad = _repoNacionalidad.DetalleDe(viewModel.NacionalidadId);
                if (nacionalidad == null)
                {
                    ModelState.AddModelError("NacionalidadId", "Nacionalidad no válida");
                    viewModel.Nacionalidades = _repoNacionalidad.Obtener().ToList();
                    return View(viewModel);
                }

                var usuario = new Usuario
                {
                    NombreUsuario = viewModel.NombreUsuario,
                    Gmail = viewModel.Gmail,
                    Contrasenia = viewModel.Contrasenia,
                    nacionalidad = nacionalidad
                };

                _repoUsuario.Alta(usuario);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear usuario: {ex.Message}");
            }
        }

        viewModel.Nacionalidades = _repoNacionalidad.Obtener().ToList();
        return View(viewModel);
    }
}