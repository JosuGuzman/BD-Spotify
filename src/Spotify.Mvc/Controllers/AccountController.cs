using Spotify.Core.Entidades;
using Spotify.Mvc.Models;

namespace Spotify.Mvc.Controllers;

public class AccountController : Controller
{
    private readonly IRepoUsuario _repoUsuario;
    private readonly IRepoNacionalidad _repoNacionalidad;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IRepoUsuario repoUsuario,
        IRepoNacionalidad repoNacionalidad,
        ILogger<AccountController> logger)
    {
        _repoUsuario = repoUsuario;
        _repoNacionalidad = repoNacionalidad;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var usuario = await _repoUsuario.ObtenerPorEmailAsync(model.Email);
            
            if (usuario == null || !BCrypt.Verify(model.Contrasenia, usuario.Contrasenia))
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas");
                return View(model);
            }

            if (!usuario.EstaActivo)
            {
                ModelState.AddModelError(string.Empty, "Cuenta desactivada");
                return View(model);
            }

            await CrearSesion(usuario);
            
            _logger.LogInformation($"Usuario {usuario.Email} inició sesión");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            
            return usuario.IdRol == 3 ? 
                RedirectToAction("Dashboard", "Admin") : 
                RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en Login");
            ModelState.AddModelError(string.Empty, "Error interno del servidor");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        var nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
        var model = new RegisterModel
        {
            Nacionalidades = nacionalidades
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
            return View(model);
        }

        try
        {
            // Verificar si el email ya existe
            var existeUsuario = await _repoUsuario.ObtenerPorEmailAsync(model.Email);
            if (existeUsuario != null)
            {
                ModelState.AddModelError("Email", "El email ya está registrado");
                model.Nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
                return View(model);
            }

            var usuario = new Usuario
            {
                NombreUsuario = model.NombreUsuario,
                Email = model.Email,
                Contrasenia = BCrypt.HashPassword(model.Contrasenia),
                IdNacionalidad = model.IdNacionalidad,
                IdRol = 2, // Usuario registrado
                EstaActivo = true,
                FechaRegistro = DateTime.Now
            };

            await _repoUsuario.InsertarAsync(usuario);
            
            // Crear sesión automáticamente
            await CrearSesion(usuario);
            
            _logger.LogInformation($"Nuevo usuario registrado: {usuario.Email}");
            
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en Register");
            ModelState.AddModelError(string.Empty, "Error al registrar usuario");
            model.Nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var usuario = await _repoUsuario.ObtenerPorIdAsync(userId);
        
        if (usuario == null)
            return NotFound();

        var nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
        
        var model = new ProfileModel
        {
            IdUsuario = usuario.IdUsuario,
            NombreUsuario = usuario.NombreUsuario,
            Email = usuario.Email,
            IdNacionalidad = usuario.IdNacionalidad,
            FotoPerfil = usuario.FotoPerfil,
            Nacionalidades = nacionalidades
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(ProfileModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
            return View("Profile", model);
        }

        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var usuario = await _repoUsuario.ObtenerPorIdAsync(userId);
            
            if (usuario == null)
                return NotFound();

            usuario.NombreUsuario = model.NombreUsuario;
            usuario.IdNacionalidad = model.IdNacionalidad;
            
            // Manejar subida de foto de perfil
            if (model.FotoPerfil != null)
            {
                var fileName = await GuardarArchivoAsync(model.FotoPerfil, "perfiles");
                usuario.FotoPerfil = fileName;
            }

            await _repoUsuario.ActualizarAsync(usuario);
            
            TempData["SuccessMessage"] = "Perfil actualizado exitosamente";
            return RedirectToAction("Profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar perfil");
            TempData["ErrorMessage"] = "Error al actualizar perfil";
            model.Nacionalidades = await _repoNacionalidad.ObtenerTodosAsync();
            return View("Profile", model);
        }
    }

    private async Task CrearSesion(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.IdRol.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }

    private async Task<string> GuardarArchivoAsync(IFormFile archivo, string subcarpeta)
    {
        var uploadsFolder = Path.Combine("wwwroot", "uploads", subcarpeta);
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        return $"/uploads/{subcarpeta}/{uniqueFileName}";
    }
}