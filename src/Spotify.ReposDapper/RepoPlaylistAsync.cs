
namespace Spotify.ReposDapper;

public class RepoPlaylistAsync : RepoGenerico, IRepoPlaylistAsync
{
    public RepoPlaylistAsync(IDbConnection conexion) 
        : base(conexion) {}

    public async Task<PlayList> AltaAsync(PlayList playlist)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidPlaylist", direction: ParameterDirection.Output);
        parametros.Add("@unNombre", playlist.Nombre);
        parametros.Add("@unidUsuario", playlist.usuario.idUsuario);


        await _conexion.ExecuteAsync("altaPlaylist", parametros, commandType: CommandType.StoredProcedure);

        playlist.idPlaylist = parametros.Get<uint>("@unidPlaylist");

        return playlist;
    }

    public async Task<PlayList?> DetalleDeAsync(uint idPlaylist)
    {
        var BuscarPlayListPorId = @"SELECT * FROM Playlist WHERE idPlaylist = @idPlaylist";

        var Buscar = await _conexion.QueryFirstOrDefaultAsync<PlayList>(BuscarPlayListPorId, new {idPlaylist});

        return Buscar;
    }

    public IList<PlayList> Obtener() => EjecutarSPConReturnDeTipoLista<PlayList>("ObtenerPlayLists").ToList();
}