using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify.Core
{
    public class Artista
    {
        public int idArtista {get;set;}
        public required string NombreArtistico {get;set;}
        public required string Nombre {get;set;}
        public required string Apellido {get;set;}
    }
}