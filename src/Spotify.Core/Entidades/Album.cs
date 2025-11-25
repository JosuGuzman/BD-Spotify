using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Spotify.Core;

public class Album
{
    [Key]
    public uint idAlbum { get; set; }

    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(45, ErrorMessage = "El título no puede exceder 45 caracteres")]
    public required string Titulo { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaLanzamiento { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "El artista es requerido")]
    public required Artista Artista { get; set; }

    public string Portada { get; set; } = "default_album.png";

    // Propiedades de navegación
    [JsonIgnore]
    public virtual ICollection<Cancion>? Canciones { get; set; }

    // Propiedades calculadas
    public int TotalCanciones => Canciones?.Count ?? 0;
    
    public TimeSpan DuracionTotal => Canciones?.Aggregate(TimeSpan.Zero, 
        (total, cancion) => total + cancion.Duracion) ?? TimeSpan.Zero;
        
    public bool EsReciente => FechaLanzamiento >= DateTime.Now.AddMonths(-6);
}