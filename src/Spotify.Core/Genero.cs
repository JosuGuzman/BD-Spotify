namespace Spotify.Core;

public class Genero
{
    public byte IdGenero {get; set;}
    public required string Generos {get; set;}
    
    public Genero (byte idGenero, string generos)
    {
        IdGenero = idGenero;
        Generos = generos;
    }
}