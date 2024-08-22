namespace Spotify.ReposDapper;
public class RepoCancion : RepoGenerico, IRepoCancion
{
    public RepoCancion(IDbConnection conexion)
        : base(conexion) {  }

    public void Alta(Cancion cancion)
    {
        
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