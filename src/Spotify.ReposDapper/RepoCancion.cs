using System.Data;
using Spotify.Core;
using Spotify.Core.Persistencia;

namespace Spotify.ReposDapper;

public class RepoCancion : RepoGenerico, IRepoCancion
{
    public RepoCancion(IDbConnection conexion) : base(conexion)
    {
    }

    public void Alta(Cancion cancion)
    {
        throw new NotImplementedException();
    }

    public IList<Cancion> Buscar(string cadena)
    {
        throw new NotImplementedException();
    }

    public Cancion? DetalleDe(int id)
    {
        throw new NotImplementedException();
    }
}