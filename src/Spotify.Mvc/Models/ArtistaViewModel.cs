namespace Spotify.Mvc.Models;

public class ArtistDetailModel
{
    public Artista Artista { get; set; } = null!;
    public IEnumerable<Album> Albumes { get; set; } = new List<Album>();
    public IEnumerable<Cancion> Canciones { get; set; } = new List<Cancion>();
}

public class ArtistaCreateViewModel
{
    public string NombreArtistico { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
}


    public class ArtistModel
    {
        public int IdArtista { get; set; }

        [Required(ErrorMessage = "El nombre artístico es requerido")]
        [StringLength(150, ErrorMessage = "El nombre artístico no puede exceder 150 caracteres")]
        public string NombreArtistico { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El nombre real no puede exceder 100 caracteres")]
        public string? NombreReal { get; set; }

        [StringLength(100, ErrorMessage = "El apellido real no puede exceder 100 caracteres")]
        public string? ApellidoReal { get; set; }

        public string? Biografia { get; set; }

        [Display(Name = "Foto del artista")]
        public IFormFile? FotoArtista { get; set; }

        [Required(ErrorMessage = "La nacionalidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una nacionalidad válida")]
        public int IdNacionalidad { get; set; }
    }