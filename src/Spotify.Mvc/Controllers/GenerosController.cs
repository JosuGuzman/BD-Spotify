using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Spotify.ReposDapper;
using Spotify.Core.Entidades;
using Spotify.Mvc.Models;

namespace Spotify.Mvc.Controllers;

[Authorize(Roles = "3")]
public class GenreController : Controller
{
    private readonly IRepoGenero _repoGenero;
    private readonly ILogger<GenreController> _logger;

    public GenreController(
        IRepoGenero repoGenero,
        ILogger<GenreController> logger)
    {
        _repoGenero = repoGenero;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var generos = await _repoGenero.ObtenerTodosAsync();
            return View(generos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener géneros");
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GeneroModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var genero = new Genero
            {
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                EstaActivo = true
            };

            await _repoGenero.InsertarAsync(genero);
            
            TempData["SuccessMessage"] = "Género creado exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear género");
            TempData["ErrorMessage"] = "Error al crear género";
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(byte id)
    {
        try
        {
            var genero = await _repoGenero.ObtenerPorIdAsync(id);
            if (genero == null)
                return NotFound();

            var model = new GeneroModel
            {
                IdGenero = genero.IdGenero,
                Nombre = genero.Nombre,
                Descripcion = genero.Descripcion
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cargar edición de género {id}");
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(byte id, GeneroModel model)
    {
        if (id != model.IdGenero)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var genero = await _repoGenero.ObtenerPorIdAsync(id);
            if (genero == null)
                return NotFound();

            genero.Nombre = model.Nombre;
            genero.Descripcion = model.Descripcion;

            await _repoGenero.ActualizarAsync(genero);
            
            TempData["SuccessMessage"] = "Género actualizado exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar género {id}");
            TempData["ErrorMessage"] = "Error al actualizar género";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(byte id)
    {
        try
        {
            await _repoGenero.EliminarAsync(id);
            TempData["SuccessMessage"] = "Género eliminado exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar género {id}");
            TempData["ErrorMessage"] = "Error al eliminar género";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(byte id)
    {
        try
        {
            var genero = await _repoGenero.ObtenerPorIdAsync(id);
            if (genero == null)
                return Json(new { success = false, message = "Género no encontrado" });

            genero.EstaActivo = !genero.EstaActivo;
            await _repoGenero.ActualizarAsync(genero);

            return Json(new { success = true, status = genero.EstaActivo });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cambiar estado de género {id}");
            return Json(new { success = false, message = "Error interno" });
        }
    }
}