namespace Spotify.ReposDapper;

public class RepoPlaylist : RepoGenerico, IRepoPlaylist
{
    public RepoPlaylist(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(PlayList playlist)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidPlaylist", direction: ParameterDirection.Output);
        parametros.Add("@unNombre", playlist.Nombre);
        parametros.Add("@unidUsuario", playlist.usuario.idUsuario);


        _conexion.Execute("altaPlaylist", parametros, commandType: CommandType.StoredProcedure);

        playlist.idPlaylist = parametros.Get<uint>("@unidPlaylist");

        return playlist.idPlaylist;
    }

    public void Eliminar(uint idPlaylist)
    {
        string eliminarPlayList = @"DELETE FROM Playlist WHERE idPlayList = @idPlayList";
        _conexion.Execute(eliminarPlayList, new {idPlaylist});
    }

    public IList<PlayList> Obtener ()
    {
        string consultarPlaylists = @"SELECT * from Playlist ORDER BY Nombre ASC";
        var Playlists = _conexion.Query<PlayList>(consultarPlaylists);
        return Playlists.ToList();
    }
    
} 
