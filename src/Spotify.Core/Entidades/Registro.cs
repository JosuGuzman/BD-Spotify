using System;

namespace Spotify.Core.Entidades;

public class Suscripcion
{
    public uint IdSuscripcion { get; set; }
    public uint IdUsuario { get; set; }
    public uint IdTipoSuscripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; }

    // Propiedades de navegación
    public virtual Usuario Usuario { get; set; }
    public virtual TipoSuscripcion TipoSuscripcion { get; set; }
}