namespace Spotify.ReposDapper;

public class RepoUsuario : RepoGenerico, IRepoUsuario
{
    public RepoUsuario(IDbConnection conexion) 
        : base(conexion) {}

    public void Alta(Album elemento)
    {
        throw new NotImplementedException();
    }

    public Album? DetalleDe(int id)
    {
        throw new NotImplementedException();
    } 

    public IList<Album> Obtener()
    {
        throw new NotImplementedException();
    }
}
