using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.Mvc.Models;

namespace Spotify.Controllers;

public class GenerosController : Controller
{
    private readonly IRepoGeneroAsync _repoGenero;

    public GenerosController(IRepoGeneroAsync repoGenero)
    {
        _repoGenero = repoGenero;
    }

    // GET: Generos
    public async Task<IActionResult> Index()
    {
        try
        {
            var generos = await _repoGenero.Obtener();
            var viewModel = generos.Select(g => new GeneroViewModel
            {
                IdGenero = g.idGenero,
                Genero = g.genero
            }).ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar géneros: {ex.Message}";
            return View(new List<GeneroViewModel>());
        }
    }

    // GET: Generos/Details/5
    public async Task<IActionResult> Details(byte id)
    {
        try
        {
            var genero = await _repoGenero.DetalleDeAsync(id);
            if (genero == null)
            {
                TempData["ErrorMessage"] = "Género no encontrado";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new GeneroViewModel
            {
                IdGenero = genero.idGenero,
                Genero = genero.genero
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar detalles: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Generos/Create
    public IActionResult Create()
    {
        return View(new GeneroCreateViewModel());
    }

    // POST: Generos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GeneroCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        try
        {
            var genero = new Genero { genero = viewModel.Genero };
            var resultado = await _repoGenero.AltaAsync(genero);
            
            TempData["SuccessMessage"] = $"Género '{resultado.genero}' creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al crear género: {ex.Message}");
            return View(viewModel);
        }
    }

    // POST: Generos/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(byte id)
    {
        try
        {
            await _repoGenero.EliminarAsync(id);
            TempData["SuccessMessage"] = "Género eliminado correctamente";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al eliminar género: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}