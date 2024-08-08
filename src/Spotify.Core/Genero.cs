namespace Spotify.Core;

public class Genero
{
    public sbyte IdGenero {get; set;}
    public required string Generos {get; set;}
    
    public Genero (sbyte idGenero, string generos)
    {
        IdGenero = idGenero;
        Generos = generos;
    }
}