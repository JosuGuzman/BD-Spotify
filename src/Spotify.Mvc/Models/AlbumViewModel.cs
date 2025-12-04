using Spotify.Core;
namespace Spotify.Mvc.Models;

public class AlbumModel
{
    public int IdAlbum { get; set; }

    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Display(Name = "Fecha de lanzamiento")]
    [DataType(DataType.Date)]
    public DateTime? FechaLanzamiento { get; set; }

    [Required(ErrorMessage = "El artista es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un artista válido")]
    public int IdArtista { get; set; }

    [Display(Name = "Portada del álbum")]
    public IFormFile? Portada { get; set; }

    public string? NombreArtista { get; set; }
}

public class AlbumCreateViewModel
{
    public string Titulo { get; set; } = string.Empty;
    public uint ArtistaId { get; set; }
    public List<Artista>? Artistas { get; set; }
}

public class AlbumDetailModel
{
    public Album Album { get; set; } = null!;
    public Artista Artista { get; set; } = null!;
    public IEnumerable<Cancion> Canciones { get; set; } = new List<Cancion>();
}