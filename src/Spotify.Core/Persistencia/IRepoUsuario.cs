using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify.Core.Persistencia;
public interface IRepoUsuario : IAlta<Album>, IListado<Album>, IDetallePorId<Album,int>
{
    
}