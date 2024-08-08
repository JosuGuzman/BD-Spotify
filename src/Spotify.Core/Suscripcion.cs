using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify.Core
{
    public class Suscripcion
    {
        public int idSuscripcion {get;set;}
        public sbyte Duracion {get;set;}
        public sbyte Costo {get;set;}
        public required string TipoSuscripcion {get;set;}
    }
}