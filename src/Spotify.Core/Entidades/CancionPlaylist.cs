using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Spotify.Core;

public class CancionPlaylist
{
    [Required(ErrorMessage = "La playlist es requerida")]
    public required PlayList Playlist { get; set; }

    [Required(ErrorMessage = "La canción es requerida")]
    public required Cancion Cancion { get; set; }

    // Propiedades adicionales para la relación
    public DateTime FechaAgregada { get; set; } = DateTime.Now;
    
    public uint Orden { get; set; }

    // Propiedades calculadas
    [JsonIgnore]
    public string Info => $"{Cancion.Titulo} en {Playlist.Nombre}";
    
    public bool EsReciente => FechaAgregada >= DateTime.Now.AddDays(-7);
}