using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify.Core
{
    public class Playlist_Cancion
    {
        public required PlayList playlist {get;set;}
        public required Cancion cancion {get;set;}
        public DateTime FechaIngresion {get;set;} 
    }
}