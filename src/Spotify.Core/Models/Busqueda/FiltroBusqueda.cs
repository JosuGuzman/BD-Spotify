namespace Spotify.Core.Models.Busqueda;

public class FiltroBusqueda
{
    public string? Termino { get; set; }
    public byte? IdGenero { get; set; }
    public uint? IdArtista { get; set; }
    public uint? IdAlbum { get; set; }
    public int? AnoMin { get; set; }
    public int? AnoMax { get; set; }
    public TimeSpan? DuracionMin { get; set; }
    public TimeSpan? DuracionMax { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamañoPagina { get; set; } = 20;
    public string OrdenarPor { get; set; } = "relevancia";
    public bool OrdenAscendente { get; set; } = false;
}