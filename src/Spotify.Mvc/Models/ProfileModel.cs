

namespace Spotify.Mvc.Models;

public class ProfileModel
{
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 60 caracteres")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nacionalidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una nacionalidad válida")]
    public int IdNacionalidad { get; set; }

    [Display(Name = "Foto de perfil")]
    public IFormFile? FotoPerfil { get; set; }

    public string? FotoPerfilUrl { get; set; }

    public List<Nacionalidad> Nacionalidades { get; set; } = new();
}