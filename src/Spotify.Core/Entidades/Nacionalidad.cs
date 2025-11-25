using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Spotify.Core;

public class Nacionalidad
{
    [Key]
    public uint idNacionalidad { get; set; }

    [Required(ErrorMessage = "El país es requerido")]
    [StringLength(45, ErrorMessage = "El país no puede exceder 45 caracteres")]
    public required string Pais { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Usuario>? Usuarios { get; set; }
}