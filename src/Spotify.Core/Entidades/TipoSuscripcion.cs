using System.Collections.Generic;

namespace Spotify.Core.Entidades;

public class TipoSuscripcion
{
    public int IdTipoSuscripcion { get; set; }
    public string Tipo { get; set; }
    public int DuracionMeses { get; set; }
    public decimal Costo { get; set; }
    public bool EstaActivo { get; set; }

    // Propiedades de navegación
    public virtual ICollection<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
}