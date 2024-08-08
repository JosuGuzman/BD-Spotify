namespace Spotify.Core;

public class HistorialReproduccion
{
    public int IdHistorial {get;set;}
    public required string NombreUsuario {get;set;}
    public required string Gmail {get;set;}
    public required string Contraseña {get;set;}
    public required Registro Registros {get;set;}
    public required Nacionalidad Nacionalidades {get;set;} 
}