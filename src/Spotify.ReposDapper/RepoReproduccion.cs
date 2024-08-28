namespace Spotify.ReposDapper;

public class RepoReproduccion : RepoGenerico, IRepoReproduccion
{
    public RepoReproduccion(IDbConnection conexion) 
        : base(conexion) {}

    public void Alta(Reproduccion elemento)
    {
        throw new NotImplementedException();
    }

    public IList<Reproduccion> Obtener()
    {
        throw new NotImplementedException();
    }
}
