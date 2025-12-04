using System;

namespace Spotify.Core.Entidades;

public class Reproduccion
{
    public long IdHistorial { get; set; }
    public uint IdUsuario { get; set; }
    public uint IdCancion { get; set; }
    public DateTime FechaReproduccion { get; set; }
    public uint? DuracionReproducida { get; set; }
    public TimeSpan? ProgresoReproduccion { get; set; }
    public bool ReproduccionCompleta { get; set; }
    public string? Dispositivo { get; set; }

    // Propiedades de navegación
    public virtual Usuario Usuario { get; set; }
    public virtual Cancion Cancion { get; set; }
}