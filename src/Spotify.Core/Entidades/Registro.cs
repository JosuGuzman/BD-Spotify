using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Spotify.Core;

public class Registro
{
    [Key]
    public uint IdSuscripcion { get; set; }

    [Required(ErrorMessage = "El usuario es requerido")]
    public required Usuario Usuario { get; set; }

    [Required(ErrorMessage = "El tipo de suscripción es requerido")]
    public required TipoSuscripcion TipoSuscripcion { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; } = DateTime.Now;

    // Propiedades adicionales
    public DateTime? FechaRenovacion { get; set; }
    
    public bool AutoRenovacion { get; set; } = true;
    
    public string? MetodoPago { get; set; }

    // Propiedades calculadas
    public DateTime FechaExpiracion => FechaInicio.AddMonths((int)TipoSuscripcion.Duracion);
    
    public bool EstaActiva => FechaExpiracion >= DateTime.Now;
    
    public bool ExpiraProximamente => FechaExpiracion <= DateTime.Now.AddDays(7) && FechaExpiracion > DateTime.Now;
    
    public int DiasRestantes => (FechaExpiracion - DateTime.Now).Days;
    
    public string Estado => EstaActiva ? 
        (ExpiraProximamente ? "Por expirar" : "Activa") : "Expirada";
}