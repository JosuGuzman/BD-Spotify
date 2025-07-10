
namespace Spotify.ReposDapper;

public class RepoCancionAsync : RepoGenerico, IRepoCancionAsync
{
    public RepoCancionAsync(IDbConnection conexion)
        : base(conexion) {}

    public async Task<Cancion> AltaAsync(Cancion cancion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidCancion", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", cancion.Titulo);
        parametros.Add("@unDuration", cancion.Duracion);
        parametros.Add("@unidAlbum", cancion.album.idAlbum);
        parametros.Add("@unidArtista", cancion.artista.idArtista);
        parametros.Add("@unidGenero", cancion.genero.idGenero);

        await _conexion.ExecuteAsync("altaCancion", parametros, commandType: CommandType.StoredProcedure);

        cancion.idCancion = parametros.Get<uint>("@unidCancion");

        return cancion;
    }

    public async Task<Cancion?> DetalleDeAsync(uint idCancion)
    {
        var BuscarCancionPorId = @"SELECT * FROM Cancion WHERE idCancion = @idCancion";

        var Buscar = await _conexion.QueryFirstOrDefaultAsync<Cancion>(BuscarCancionPorId, new {idCancion});

        return Buscar;
    }

    public async Task<List<string>?> Matcheo(string Cadena)
    {
        var parametro = new DynamicParameters();
        parametro.Add("@InputCancion", Cadena);

        var Lista = await _conexion.QueryAsync<string>("MatcheoCancion", parametro, commandType: CommandType.StoredProcedure);

        return Lista.ToList();
    }

    public IList<Cancion> Obtener() => EjecutarSPConReturnDeTipoLista<Cancion>("ObtenerCanciones").ToList();
}