namespace Spotify.Mvc.Models;

public class SearchResultModel
{
    public string Query { get; set; } = string.Empty;
    public List<Album> Albumes { get; set; } = new();
    public List<Artista> Artistas { get; set; } = new();
    public List<Cancion> Canciones { get; set; } = new();
}

public class AdvancedSearchModel
{
    public string Query { get; set; } = string.Empty;
    public int? GeneroId { get; set; }
    public int? ArtistaId { get; set; }
    public int? Anio { get; set; }
    public int? DuracionMin { get; set; }
    public int? DuracionMax { get; set; }
}

public class AdvancedSearchResultModel
{
    public string Query { get; set; } = string.Empty;
    public IEnumerable<Cancion> Canciones { get; set; } = new List<Cancion>();
    public int Total { get; set; }
}

public class SearchByGenreModel
{
    public Genero Genero { get; set; } = null!;
    public IEnumerable<Cancion> Canciones { get; set; } = new List<Cancion>();
}

public class SearchByArtistModel
{
    public Artista Artista { get; set; } = null!;
    public IEnumerable<Cancion> Canciones { get; set; } = new List<Cancion>();
    public IEnumerable<Album> Albumes { get; set; } = new List<Album>();
}