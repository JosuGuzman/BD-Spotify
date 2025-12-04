namespace Spotify.Core.Entidades;

public class Album
{
    public int IdAlbum { get; set; }
    public string Titulo { get; set; }
    public DateTime? FechaLanzamiento { get; set; }
    public int IdArtista { get; set; }
    public string? Portada { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public bool EstaActivo { get; set; }

    // Propiedades de navegación
    public virtual Artista Artista { get; set; }
    public virtual ICollection<Cancion> Canciones { get; set; } = new List<Cancion>();
}