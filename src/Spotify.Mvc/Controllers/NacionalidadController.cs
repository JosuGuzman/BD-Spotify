using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Spotify.ReposDapper;
using Spotify.Core.Entidades;
using Spotify.Mvc.Models;

namespace Spotify.Mvc.Controllers;

[Authorize(Roles = "3")]
public class NationalityController : Controller
{
    private readonly IRepoNacionalidad _repoNacionalidad;
    private readonly ILogger<NationalityController> _logger;

    public NationalityController(
        IRepoNacionalidad repoNacionalidad,
        ILogger<NationalityController> logger)
    {
        _repoNacionalidad = repoNacionalidad;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
            return View(nacionalidades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener nacionalidades");
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
    public async Task<IActionResult> Create(NacionalidadModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var nacionalidad = new Nacionalidad
            {
                Pais = model.Pais
            };

            await _repoNacionalidad.InsertarAsync(nacionalidad);
            
            TempData["SuccessMessage"] = "Nacionalidad creada exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nacionalidad");
            TempData["ErrorMessage"] = "Error al crear nacionalidad";
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var nacionalidad = await _repoNacionalidad.ObtenerPorIdAsync(id);
            if (nacionalidad == null)
                return NotFound();

            var model = new NacionalidadModel
            {
                IdNacionalidad = nacionalidad.IdNacionalidad,
                Pais = nacionalidad.Pais
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al cargar edición de nacionalidad {id}");
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NacionalidadModel model)
    {
        if (id != model.IdNacionalidad)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var nacionalidad = await _repoNacionalidad.ObtenerPorIdAsync(id);
            if (nacionalidad == null)
                return NotFound();

            nacionalidad.Pais = model.Pais;

            await _repoNacionalidad.ActualizarAsync(nacionalidad);
            
            TempData["SuccessMessage"] = "Nacionalidad actualizada exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar nacionalidad {id}");
            TempData["ErrorMessage"] = "Error al actualizar nacionalidad";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repoNacionalidad.EliminarAsync(id);
            TempData["SuccessMessage"] = "Nacionalidad eliminada exitosamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar nacionalidad {id}");
            TempData["ErrorMessage"] = "Error al eliminar nacionalidad";
            return RedirectToAction("Index");
        }
    }
}