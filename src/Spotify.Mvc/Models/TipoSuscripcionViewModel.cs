namespace Spotify.Mvc.Models;

public class TipoSuscripcionModel
{
    public int IdTipoSuscripcion { get; set; }

    [Required(ErrorMessage = "El tipo es requerido")]
    [StringLength(100, ErrorMessage = "El tipo no puede exceder 100 caracteres")]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La duración es requerida")]
    [Range(1, 36, ErrorMessage = "La duración debe estar entre 1 y 36 meses")]
    [Display(Name = "Duración (meses)")]
    public int DuracionMeses { get; set; }

    [Required(ErrorMessage = "El costo es requerido")]
    [Range(0, 999999.99, ErrorMessage = "El costo debe ser un valor positivo")]
    [DataType(DataType.Currency)]
    public decimal Costo { get; set; }
}