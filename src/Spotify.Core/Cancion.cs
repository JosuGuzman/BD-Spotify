using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify.Core
{
    public class Cancion
    {
        public int idCacion {get;set;}
        public required string Titulo {get;set;} 
        public DateTime duracion {get;set;}
        public required Artista artista {get;set;}
        public required Album album {get;set;}
        public required Genero genero {get;set;}
    }
}