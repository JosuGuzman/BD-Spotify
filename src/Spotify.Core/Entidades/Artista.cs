namespace Spotify.Core.Entidades;

public class Artista
{
    public uint IdArtista { get; set; }
    public string NombreArtistico { get; set; }
    public string? NombreReal { get; set; }
    public string? ApellidoReal { get; set; }
    public string? Biografia { get; set; }
    public string? FotoArtista { get; set; }
    public bool EstaActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public int? IdNacionalidad { get; set; }

    // Propiedades de navegación
    public virtual Nacionalidad Nacionalidad { get; set; }
    public virtual ICollection<Album> Albumes { get; set; } = new List<Album>();
    public virtual ICollection<Cancion> Canciones { get; set; } = new List<Cancion>();
}