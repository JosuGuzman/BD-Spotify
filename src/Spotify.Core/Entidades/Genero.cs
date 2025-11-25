using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Spotify.Core;

public class Genero
{
    [Key]
    public byte idGenero { get; set; }

    [Required(ErrorMessage = "El género es requerido")]
    [StringLength(45, ErrorMessage = "El género no puede exceder 45 caracteres")]
    public required string Nombre { get; set; } // Cambiado de 'genero' a 'Nombre'

    public string Descripcion { get; set; } = string.Empty;

    // Propiedades de navegación
    [JsonIgnore]
    public virtual ICollection<Cancion>? Canciones { get; set; }

    // Propiedad calculada
    public int TotalCanciones => Canciones?.Count ?? 0;
}