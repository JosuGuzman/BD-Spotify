using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;


namespace Spotify.Core;

public class Usuario
{
    [Key]
    public uint idUsuario { get; set; }
    
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(45, ErrorMessage = "El nombre de usuario no puede exceder 45 caracteres")]
    public required string NombreUsuario { get; set; }

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(45, ErrorMessage = "El email no puede exceder 45 caracteres")]
    public required string Email { get; set; } // Cambiado de Gmail a Email para coincidir con BD
    
    [Required(ErrorMessage = "La contraseña es requerida")]
    [JsonIgnore] // No exponer la contraseña en respuestas JSON
    public required string Contrasenia { get; set; }
    
    [Required(ErrorMessage = "La nacionalidad es requerida")]
    public required Nacionalidad Nacionalidad { get; set; }
    
    // Propiedades de navegación
    
    [JsonIgnore]
    public virtual ICollection<PlayList>? Playlists { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Reproduccion>? Reproducciones { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Registro>? Suscripciones { get; set; }
    
    // Propiedades calculadas
    
    public bool EstaActivo => Suscripciones?.Any(s => 
        s.FechaInicio.AddMonths((int)s.TipoSuscripcion.Duracion) >= DateTime.Now) ?? false;
}