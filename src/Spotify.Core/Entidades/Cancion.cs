using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Spotify.Core;

public class Cancion
{
    [Key]
    public uint idCancion { get; set; }

    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(45, ErrorMessage = "El título no puede exceder 45 caracteres")]
    public required string Titulo { get; set; }

    public TimeSpan Duracion { get; set; }

    [Required(ErrorMessage = "El álbum es requerido")]
    public required Album Album { get; set; }

    [Required(ErrorMessage = "El género es requerido")]
    public required Genero Genero { get; set; }

    [Required(ErrorMessage = "El artista es requerido")]
    public required Artista Artista { get; set; }

    public string ArchivoMP3 { get; set; } = string.Empty;

    // Propiedades de navegación
    [JsonIgnore]
    public virtual ICollection<PlayList>? Playlists { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Reproduccion>? Reproducciones { get; set; }

    // Propiedades calculadas
    public string DuracionFormateada => $"{(int)Duracion.TotalMinutes}:{Duracion.Seconds:D2}";
    
    public int TotalReproducciones => Reproducciones?.Count ?? 0;
    
    public bool TieneArchivo => !string.IsNullOrEmpty(ArchivoMP3) && ArchivoMP3 != "default.mp3";
}