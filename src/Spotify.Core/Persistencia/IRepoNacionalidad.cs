using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify.Core.Persistencia;
public interface IRepoNacionalidad : IAlta<Nacionalidad>, IListado<Nacionalidad>, IDetallePorId<Nacionalidad,int>
{
    
}