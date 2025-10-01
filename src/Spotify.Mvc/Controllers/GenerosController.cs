using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;
using Spotify.Mvc.Models;

namespace Spotify.Controllers;

public class GenerosController : Controller
{
    private readonly IRepoGenero _repoGenero;

    public GenerosController(IRepoGenero repoGenero)
    {
        _repoGenero = repoGenero;
    }

    public IActionResult Index()
    {
        var generos = _repoGenero.Obtener();
        var viewModel = generos.Select(g => new GeneroViewModel
        {
            IdGenero = g.idGenero,
            Genero = g.genero
        }).ToList();

        return View(viewModel);
    }

    public IActionResult Details(byte id)
    {
        var genero = _repoGenero.DetalleDe(id);
        if (genero == null)
            return NotFound();

        var viewModel = new GeneroViewModel
        {
            IdGenero = genero.idGenero,
            Genero = genero.genero
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        return View(new GeneroCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(GeneroCreateViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var genero = new Genero { genero = viewModel.Genero };
                _repoGenero.Alta(genero);
                
                TempData["SuccessMessage"] = "Género creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear género: {ex.Message}");
            }
        }
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(byte id)
    {
        try
        {
            _repoGenero.Eliminar(id);
            TempData["SuccessMessage"] = "Género eliminado correctamente";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al eliminar género: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}