namespace Spotify.ReposDapper;

public class RepoTipoSuscripcion : RepoGenerico, IRepoTipoSuscripcion
{
    public RepoTipoSuscripcion(IDbConnection conexion) 
        : base(conexion) {}

    public void Alta(TipoSuscripcion elemento)
    {
        throw new NotImplementedException();
    }

    public IList<TipoSuscripcion> Obtener()
    { 
        throw new NotImplementedException();
    }
}
 