namespace Spotify.Mvc.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 60 caracteres")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    [DataType(DataType.Password)]
    public string Contrasenia { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
    [DataType(DataType.Password)]
    [Compare("Contrasenia", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmarContrasenia { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nacionalidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una nacionalidad válida")]
    public int IdNacionalidad { get; set; }

    public List<Nacionalidad> Nacionalidades { get; set; } = new();
}
    