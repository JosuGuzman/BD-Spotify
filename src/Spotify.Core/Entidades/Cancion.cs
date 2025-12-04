namespace Spotify.Core.Entidades;

public class Cancion
{
    public int IdCancion { get; set; }
    public string Titulo { get; set; }
    public int DuracionSegundos { get; set; }
    public int IdAlbum { get; set; }
    public int IdArtista { get; set; }
    public byte IdGenero { get; set; }
    public string ArchivoMP3 { get; set; }
    public long ContadorReproducciones { get; set; }
    public bool EstaActiva { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }

    // Propiedades de navegación
    public virtual Album Album { get; set; }
    public virtual Artista Artista { get; set; }
    public virtual Genero Genero { get; set; }
    public virtual ICollection<CancionPlaylist> CancionPlaylists { get; set; } = new List<CancionPlaylist>();
    public virtual ICollection<Reproduccion> HistorialReproducciones { get; set; } = new List<Reproduccion>();
    public virtual ICollection<MeGusta> MeGustas { get; set; } = new List<MeGusta>();
}