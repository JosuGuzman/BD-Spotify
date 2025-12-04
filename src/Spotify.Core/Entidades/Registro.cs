using System;

namespace Spotify.Core.Entidades;

public class Suscripcion
{
    public int IdSuscripcion { get; set; }
    public int IdUsuario { get; set; }
    public int IdTipoSuscripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; }

    // Propiedades de navegación
    public virtual Usuario Usuario { get; set; }
    public virtual TipoSuscripcion TipoSuscripcion { get; set; }
}