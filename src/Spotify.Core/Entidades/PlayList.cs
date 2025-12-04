namespace Spotify.Core.Entidades;

public class Playlist
{
    public uint IdPlaylist { get; set; }
    public uint IdUsuario { get; set; }
    public string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public bool EsPublica { get; set; }
    public bool EsSistema { get; set; }
    public bool EstaActiva { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }

    // Propiedades de navegación
    public virtual Usuario Usuario { get; set; }
    public virtual ICollection<CancionPlaylist> CancionPlaylists { get; set; } = new List<CancionPlaylist>();
    public virtual ICollection<Cancion> Canciones { get; set; } = new List<Cancion>();
}