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

    public PlayList DetalleDe(uint idPlaylist)
    {
        var BuscarPlayListPorId = @"SELECT * FROM Playlist WHERE idPlaylist = @idPlaylist";

        var Buscar = _conexion.QueryFirstOrDefault<PlayList>(BuscarPlayListPorId, new {idPlaylist});

        return Buscar; 
    }

    public IList<PlayList> Obtener ()
    {
        var PlayLists = _conexion.Query<PlayList>("ObtenerPlayLists", commandType: CommandType.StoredProcedure);
        
        return PlayLists.ToList();
    }
    
} 
