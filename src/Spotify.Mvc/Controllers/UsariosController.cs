using Microsoft.AspNetCore.Mvc;
using Spotify.Core;
using Spotify.Core.Persistencia;
using MinimalAPI.DTOs;

namespace Spotify.Mvc.Controllers;

public class UsuariosController : Controller
{
    private readonly IRepoUsuario _repo;

    public UsuariosController(IRepoUsuario repo)
    {
        _repo = repo;
    }

    public IActionResult Index()
    {
        var usuarios = _repo.Obtener();
        var dtoList = usuarios.Select(u => new UsuarioOutputDTO
        {
            idUsuario = u.idUsuario,
            NombreUsuario = u.NombreUsuario,
            Gmail = u.Gmail,
            Nacionalidad = u.nacionalidad?.Pais ?? "Desconocida"
        });
        return View(dtoList);
    }

    public IActionResult Details(uint id)
    {
        var usuario = _repo.DetalleDe(id);
        if (usuario is null) return NotFound();

        var dto = new UsuarioOutputDTO
        {
            idUsuario = usuario.idUsuario,
            NombreUsuario = usuario.NombreUsuario,
            Gmail = usuario.Gmail,
            Nacionalidad = usuario.nacionalidad?.Pais ?? "Desconocida"
        };
        return View(dto);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UsuarioInputDTO dto)
    {
        if (ModelState.IsValid)
        {
            var usuario = new Usuario
            {
                NombreUsuario = dto.NombreUsuario,
                Gmail = dto.Gmail,
                Contrasenia = dto.Contrasenia,
                nacionalidad = new Nacionalidad
                {
                    idNacionalidad = dto.Nacionalidad,
                    Pais = string.Empty
                }
            };

            _repo.Alta(usuario);
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }
}