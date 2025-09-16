namespace Spotify.Core;

public class Genero
{
    public byte idGenero {get; set;}
    public required string genero {get; set;}
}

public class CancionPlaylist
{
    public required Album album {get;set;}
    public required Cancion cancion {get;set;}
}

public class Nacionalidad
{
    public uint idNacionalidad {get;set;}
    public required string Pais {get;set;}
}

public class Usuario
{
    public uint idUsuario {get;set;}
    public required string NombreUsuario {get;set;}
    public required string Gmail {get;set;}
    public required string Contrasenia {get;set;}
    public required Nacionalidad nacionalidad {get;set;}
}

public class Reproduccion
{
    public uint IdHistorial {get; set;}
    public required Usuario usuario {get;set;}
    public required Cancion cancion {get; set;}
    public DateTime FechaReproduccion {get; set;}
}

public class Registro
{
    public uint idSuscripcion {get; set;}
    public required Usuario usuario {get;set;}
    public required TipoSuscripcion tipoSuscripcion {get;set;}
    public DateTime FechaInicio {get;set;}
}

public class Cancion
{
    public uint idCancion {get;set;}
    public required string Titulo {get;set;}
    public TimeSpan Duracion {get;set;}
    public required Album album {get;set;}
    public required Genero genero {get;set;}
    public required Artista artista {get;set;}
}

public class PlayList
{
    public uint idPlaylist {get;set;}
    public required string Nombre {get;set;}
    public required Usuario usuario {get;set;}
    public required List<Cancion> Canciones {get;set;}
}

public class Album
{
    public uint idAlbum {get;set;}
    public required string Titulo {get;set;}
    public DateTime FechaLanzamiento {get;set;}
    public required Artista artista {get;set;}
}

public class TipoSuscripcion
{
    public uint IdTipoSuscripcion {get;set;}
    public byte Duracion {get;set;}
    public byte Costo {get;set;}
    public required string Tipo {get;set;}
}

public class Artista
{
    public uint idArtista {get;set;}
    public required string NombreArtistico {get;set;}
    public required string Nombre {get;set;}
    public required string Apellido {get;set;}
}