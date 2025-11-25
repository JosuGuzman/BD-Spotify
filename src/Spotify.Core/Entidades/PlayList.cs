using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Spotify.Core;

public class PlayList
{
    [Key]
    public uint idPlaylist { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(20, ErrorMessage = "El nombre no puede exceder 20 caracteres")]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = "El usuario es requerido")]
    public required Usuario Usuario { get; set; }

    public virtual ICollection<Cancion> Canciones { get; set; } = new List<Cancion>();

    // Propiedades calculadas
    public int TotalCanciones => Canciones.Count;
    
    public TimeSpan DuracionTotal => Canciones.Aggregate(TimeSpan.Zero, 
        (total, cancion) => total + cancion.Duracion);
        
    public DateTime? FechaCreacion { get; set; } = DateTime.Now;
    
    public bool EsFavoritos => Nombre.Equals("Tus Megusta", StringComparison.OrdinalIgnoreCase);
}