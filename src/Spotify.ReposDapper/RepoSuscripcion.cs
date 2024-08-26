namespace Spotify.ReposDapper;

public class RepoSuscripcion : RepoGenerico, IRepoSuscripcion
{
    public RepoSuscripcion(IDbConnection conexion) 
        : base(conexion) {}

    public void Alta(Suscripcion elemento)
    {
        throw new NotImplementedException();
    }

    public Suscripcion? DetalleDe(int id)
    {
        throw new NotImplementedException();
    }

    public IList<Suscripcion> Obtener()
    {
        throw new NotImplementedException();
    }
}
