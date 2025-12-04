namespace Spotify.Mvc.Models;

public class SongModel
{
    public int IdCancion { get; set; }

    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La duración es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser mayor a 0")]
    [Display(Name = "Duración (segundos)")]
    public int DuracionSegundos { get; set; }

    [Required(ErrorMessage = "El álbum es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un álbum válido")]
    [Display(Name = "Álbum")]
    public int IdAlbum { get; set; }

    [Required(ErrorMessage = "El artista es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un artista válido")]
    [Display(Name = "Artista")]
    public int IdArtista { get; set; }

    [Required(ErrorMessage = "El género es requerido")]
    [Range(1, byte.MaxValue, ErrorMessage = "Seleccione un género válido")]
    [Display(Name = "Género")]
    public byte IdGenero { get; set; }

    [Required(ErrorMessage = "El archivo MP3 es requerido")]
    [Display(Name = "Archivo MP3")]
    public IFormFile? ArchivoMP3 { get; set; }

    // New properties
    public string? ArtistaNombre { get; set; }
    public string? Genero { get; set; }

    public IEnumerable<Album>? Albumes { get; set; }
    public IEnumerable<Artista>? Artistas { get; set; }
    public IEnumerable<Genero>? Generos { get; set; }
}

public class SongCreateModel
{
    public IEnumerable<Album> Albumes { get; set; } = new List<Album>();
    public IEnumerable<Artista> Artistas { get; set; } = new List<Artista>();
    public IEnumerable<Genero> Generos { get; set; } = new List<Genero>();
}

public class SongListModel : PaginatedModel<Cancion>
{
    public IEnumerable<Genero> Generos { get; set; } = new List<Genero>();
    public IEnumerable<Artista> Artistas { get; set; } = new List<Artista>();
    public IEnumerable<Cancion> Canciones { get; internal set; }
}

public class SongDetailModel
{
    public Cancion Cancion { get; set; } = null!;
    public Album Album { get; set; } = null!;
    public Artista Artista { get; set; } = null!;
    public Genero Genero { get; set; } = null!;
}