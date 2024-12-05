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

    public IList<PlayList> Obtener () => EjecutarSPConReturnDeTipoLista<PlayList>("ObtenerPlayLists").ToList();
    
    public IList<Cancion>? DetallePlaylist(uint idPlaylist)
    {
        var consultaExistenciaPlaylist = "SELECT COUNT(*) FROM Playlist WHERE idPlaylist = @idPlaylist";
        var noExiste = _conexion.ExecuteScalar<int>(consultaExistenciaPlaylist, new { idPlaylist }) == 0;

        if (noExiste)
        {
            return null; 
        }

        var query = @"
            SELECT c.* 
            FROM Cancion c
            JOIN Cancion_Playlist cp ON c.idCancion = cp.idCancion
            WHERE cp.idPlaylist = @idPlaylist";

        var canciones = _conexion.Query<Cancion>(query, new { idPlaylist }).ToList();

        
        return canciones; 
    }
} 
