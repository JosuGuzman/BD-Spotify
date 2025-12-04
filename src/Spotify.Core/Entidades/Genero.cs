using System.Collections.Generic;

namespace Spotify.Core.Entidades;

public class Genero
{
    public string? genero;

    public byte IdGenero { get; set; }
    public string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public bool EstaActivo { get; set; }

    // Propiedades de navegación
    public virtual ICollection<Cancion> Canciones { get; set; } = new List<Cancion>();
}