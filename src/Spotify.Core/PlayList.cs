using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify.Core
{
    public class PlayList
    {
        public int IdPlaylist {get;set;}
        public required string Nombre {get;set;}
        public int IdUsuario {get;set;}
    }
}