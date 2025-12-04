using System.Collections.Generic;

namespace Spotify.Core.Entidades;

public class Rol
{
    public byte IdRol { get; set; }
    public string Nombre { get; set; }
    public string? Descripcion { get; set; }

    // Propiedades de navegación
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}