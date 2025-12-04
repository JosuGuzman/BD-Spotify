using System.Collections.Generic;

namespace Spotify.Core.Entidades;

public class Nacionalidad
{
    public int IdNacionalidad { get; set; }
    public string Pais { get; set; }

    // Propiedades de navegación
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public virtual ICollection<Artista> Artistas { get; set; } = new List<Artista>();
}