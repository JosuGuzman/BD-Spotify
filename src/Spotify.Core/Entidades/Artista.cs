using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Spotify.Core;

public class Artista
{
    [Key]
    public uint idArtista { get; set; }

    [StringLength(35, ErrorMessage = "El nombre artístico no puede exceder 35 caracteres")]
    public string? NombreArtistico { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(45, ErrorMessage = "El nombre no puede exceder 45 caracteres")]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = "El apellido es requerido")]
    [StringLength(45, ErrorMessage = "El apellido no puede exceder 45 caracteres")]
    public required string Apellido { get; set; }

    // Propiedades de navegación
    [JsonIgnore]
    public virtual ICollection<Album>? Albumes { get; set; }
        
    [JsonIgnore]
    public virtual ICollection<Cancion>? Canciones { get; set; }

    // Propiedad calculada
    public string NombreCompleto => $"{Nombre} {Apellido}";
        
    public string DisplayName => NombreArtistico ?? NombreCompleto;
}