namespace Spotify.Mvc.Models;

public class NacionalidadModel
{
    public int IdNacionalidad { get; set; }

    [Required(ErrorMessage = "El país es requerido")]
    [StringLength(100, ErrorMessage = "El país no puede exceder 100 caracteres")]
    public string Pais { get; set; } = string.Empty;
}