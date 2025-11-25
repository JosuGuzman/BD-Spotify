using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Spotify.Core;

public class Reproduccion
{
    [Key]
    public uint IdHistorial { get; set; }

    [Required(ErrorMessage = "El usuario es requerido")]
    public required Usuario Usuario { get; set; }

    [Required(ErrorMessage = "La canción es requerida")]
    public required Cancion Cancion { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime FechaReproduccion { get; set; } = DateTime.Now;

    // Propiedades adicionales para analytics
    public TimeSpan ProgresoReproduccion { get; set; }
    
    public bool ReproduccionCompleta { get; set; }
    
    public string? Dispositivo { get; set; }

    // Propiedades calculadas
    public double PorcentajeEscuchado => 
        ReproduccionCompleta ? 100 : (ProgresoReproduccion.TotalSeconds / Cancion.Duracion.TotalSeconds) * 100;
    
    public bool EsHoy => FechaReproduccion.Date == DateTime.Today;
    
    public string DispositivoFormateado => Dispositivo ?? "Web Player";
}