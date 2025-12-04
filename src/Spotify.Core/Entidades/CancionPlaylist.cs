using System;

namespace Spotify.Core.Entidades;

public class CancionPlaylist
{
    public int IdCancion { get; set; }
    public int IdPlaylist { get; set; }
    public int Orden { get; set; }
    public DateTime FechaAgregado { get; set; }

    // Propiedades de navegación
    public virtual Cancion Cancion { get; set; }
    public virtual Playlist Playlist { get; set; }
}