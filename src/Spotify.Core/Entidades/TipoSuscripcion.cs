using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Spotify.Core;

namespace Spotify.Core;

public class TipoSuscripcion
{
    [Key]
    public uint IdTipoSuscripcion { get; set; }

    [Required(ErrorMessage = "La duración es requerida")]
    [Range(1, 12, ErrorMessage = "La duración debe estar entre 1 y 12 meses")]
    public byte Duracion { get; set; }

    [Required(ErrorMessage = "El costo es requerido")]
    [Range(1, 100, ErrorMessage = "El costo debe estar entre 1 y 100")]
    public byte Costo { get; set; }

    [Required(ErrorMessage = "El tipo es requerido")]
    [StringLength(45, ErrorMessage = "El tipo no puede exceder 45 caracteres")]
    public required string Tipo { get; set; }

    // Propiedades de navegación
    [JsonIgnore]
    public virtual ICollection<Registro>? Registros { get; set; }

    // Propiedades calculadas
    public decimal CostoMensual => Costo / (decimal)Duracion;
    
    public string DescripcionCompleta => $"{Tipo} - ${Costo} por {Duracion} mes(es)";
    
    public bool EsPopular => Costo <= 10; // Suscripciones económicas son populares
}