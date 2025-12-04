using System;

namespace Spotify.Core.Entidades;

public class CancionPlaylist
{
    public uint IdCancion { get; set; }
    public uint IdPlaylist { get; set; }
    public uint Orden { get; set; }
    public DateTime FechaAgregado { get; set; }

    // Propiedades de navegación
    public virtual Cancion Cancion { get; set; }
    public virtual Playlist Playlist { get; set; }
}