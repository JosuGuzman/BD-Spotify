using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify.Core
{
    public class HistorialReproduccion
    {
        public int idHistorial {get;set;}
        public required string NombreUsuario {get;set;}
        public required string Gmail {get;set;}
        public required string Contraseña {get;set;}
        public Registro registro {get;set;}
        public Nacionalidad nacionalidad {get;set;} 
    }
}