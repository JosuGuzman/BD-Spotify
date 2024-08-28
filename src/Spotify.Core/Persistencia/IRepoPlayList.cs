using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify.Core.Persistencia;
public interface IRepoPlaylist : IAlta<PlayList>, IListado<PlayList>
{
    
}