namespace Spotify.Core
{
    public class Registro
    {
        public int idUsuario {get;set;}
        public int idSuscripcion {get;set;}
        public required Suscripcion suscripcion {get;set;}
        public DateTime FechaInicio {get;set;}
    }
}