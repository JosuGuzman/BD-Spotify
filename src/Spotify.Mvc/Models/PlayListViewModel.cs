namespace Spotify.Mvc.Models;

public class PlaylistModel
{
    public int IdPlaylist { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    [DataType(DataType.MultilineText)]
    public string? Descripcion { get; set; }

    [Display(Name = "¿Es pública?")]
    public bool EsPublica { get; set; } = true;
}