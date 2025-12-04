namespace Spotify.Mvc.Models;

public class GeneroModel
{
    public byte IdGenero { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres")]
    [DataType(DataType.MultilineText)]
    public string? Descripcion { get; set; }
}

public class GeneroCreateViewModel
{
    public string Genero { get; set; } = string.Empty;
}