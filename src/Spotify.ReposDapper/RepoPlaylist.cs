namespace Spotify.ReposDapper;

public class RepoPlaylist : RepoGenerico, IRepoPlaylist
{
    public RepoPlaylist(IDbConnection conexion) 
        : base(conexion) {}

    public void Alta(PlayList elemento)
    {
        throw new NotImplementedException();
    }

    public PlayList? DetalleDe(int id)
    {
        throw new NotImplementedException();
    }

    public IList<PlayList> Obtener()
    {
        throw new NotImplementedException();
    }
}
