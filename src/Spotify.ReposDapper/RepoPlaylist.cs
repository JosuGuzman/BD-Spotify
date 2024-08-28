namespace Spotify.ReposDapper;

public class RepoPlaylist : RepoGenerico, IRepoPlaylist
{
    public RepoPlaylist(IDbConnection conexion) 
        : base(conexion) {}

    public void Alta(PlayList playlist)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidPlaylist", direction: ParameterDirection.Output);
        parametros.Add("@unNombre", playlist.Nombre);
        parametros.Add("@unidUsuario", playlist.IdUsuario);


        _conexion.Execute("altaPlaylist", parametros, commandType: CommandType.StoredProcedure);

        playlist.IdPlaylist = parametros.Get<int>("@unidPlaylist");
    }
   
    public IList<PlayList> Obtener ()
    {
        string consultarPlaylists = @"SELECT * from PlayList ORDER BY Nombre ASC";
        var Playlists = _conexion.Query<PlayList>(consultarPlaylists);
        return Playlists.ToList();
    }
    
} 
