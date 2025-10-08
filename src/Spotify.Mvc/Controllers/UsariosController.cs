using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.Mvc.Models;
using System.Diagnostics;

namespace Spotify.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IRepoUsuarioAsync _repoUsuario;
        private readonly IRepoNacionalidadAsync _repoNacionalidad;

        public UsuariosController(
            IRepoUsuarioAsync repoUsuario, 
            IRepoNacionalidadAsync repoNacionalidad) // ✅ Este es el servicio que faltaba
        {
            _repoUsuario = repoUsuario;
            _repoNacionalidad = repoNacionalidad;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            try
            {
                var usuarios = await _repoUsuario.Obtener();
                var viewModel = new List<UsuarioViewModel>();
            
                foreach (var usuario in usuarios)
                {
                    string pais = "Desconocida";
                    if (usuario.nacionalidad != null && usuario.nacionalidad.idNacionalidad > 0)
                    {
                        var nacionalidad = await _repoNacionalidad.DetalleDeAsync(usuario.nacionalidad.idNacionalidad);
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar usuarios: {ex.Message}";
                return View(new List<UsuarioViewModel>());
            }
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(uint id)
        {
            try
            {
                var usuario = await _repoUsuario.DetalleDeAsync(id);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuario no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var pais = usuario.nacionalidad?.Pais ?? "Desconocida";
                
                if (usuario.nacionalidad?.idNacionalidad > 0 && string.IsNullOrEmpty(usuario.nacionalidad.Pais))
                {
                    var nacionalidad = await _repoNacionalidad.DetalleDeAsync(usuario.nacionalidad.idNacionalidad);
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar detalles: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Usuarios/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var nacionalidades = await _repoNacionalidad.Obtener();
                var viewModel = new UsuarioCreateViewModel
                {
                    Nacionalidades = nacionalidades.ToList()
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar formulario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    viewModel.Nacionalidades = (await _repoNacionalidad.Obtener()).ToList();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al recargar nacionalidades: {ex.Message}");
                }
                return View(viewModel);
            }

            try
            {
                var nacionalidad = await _repoNacionalidad.DetalleDeAsync(viewModel.NacionalidadId);
                if (nacionalidad == null)
                {
                    ModelState.AddModelError("NacionalidadId", "Nacionalidad no válida");
                    viewModel.Nacionalidades = (await _repoNacionalidad.Obtener()).ToList();
                    return View(viewModel);
                }

                var usuario = new Usuario
                {
                    NombreUsuario = viewModel.NombreUsuario,
                    Gmail = viewModel.Gmail,
                    Contrasenia = viewModel.Contrasenia,
                    nacionalidad = nacionalidad
                };

                var resultado = await _repoUsuario.AltaAsync(usuario);
                TempData["SuccessMessage"] = $"Usuario '{resultado.NombreUsuario}' creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear usuario: {ex.Message}");
                
                try
                {
                    viewModel.Nacionalidades = (await _repoNacionalidad.Obtener()).ToList();
                }
                catch (Exception reloadEx)
                {
                    ModelState.AddModelError("", $"Error al recargar nacionalidades: {reloadEx.Message}");
                }
                
                return View(viewModel);
            }
        }
    }
}