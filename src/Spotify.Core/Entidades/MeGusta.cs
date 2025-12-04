using System;

namespace Spotify.Core.Entidades;

public class MeGusta
{
    public uint IdUsuario { get; set; }
    public uint IdCancion { get; set; }
    public DateTime FechaMeGusta { get; set; }

    // Propiedades de navegación
    public virtual Usuario Usuario { get; set; }
    public virtual Cancion Cancion { get; set; }
}