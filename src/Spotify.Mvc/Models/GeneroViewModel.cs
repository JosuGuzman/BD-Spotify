namespace Spotify.Mvc.Models;

public class GeneroViewModel
{
    public byte IdGenero { get; set; }
    public string Genero { get; set; } = string.Empty;
}

public class GeneroCreateViewModel
{
    public string Genero { get; set; } = string.Empty;
}