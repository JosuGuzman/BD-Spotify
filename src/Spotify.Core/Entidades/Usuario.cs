namespace Spotify.Core.Entidades;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; }
    public string Email { get; set; }
    public string Contrasenia { get; set; }
    public int IdNacionalidad { get; set; }
    public byte IdRol { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string? FotoPerfil { get; set; }
    public bool EstaActivo { get; set; }
    public DateTime? UltimoAcceso { get; set; }

    // Propiedades de navegación
    public virtual Nacionalidad Nacionalidad { get; set; }
    public virtual Rol Rol { get; set; }
    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    public virtual ICollection<Reproduccion> HistorialReproducciones { get; set; } = new List<Reproduccion>();
    public virtual ICollection<MeGusta> MeGustas { get; set; } = new List<MeGusta>();
    public virtual ICollection<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
}