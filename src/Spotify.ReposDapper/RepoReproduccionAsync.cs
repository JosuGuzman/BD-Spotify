namespace Spotify.ReposDapper;

public class RepoReproduccionAsync : RepoGenerico, IRepoReproduccionAsync
{
    public RepoReproduccionAsync(IDbConnection conexion) 
        : base(conexion) {}

    public async Task<Reproduccion> AltaAsync(Reproduccion reproduccion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidHistorial", direction: ParameterDirection.Output);
        parametros.Add("@unidUsuario", reproduccion.usuario.idUsuario);
        parametros.Add("@unidCancion", reproduccion.cancion.idCancion);
        parametros.Add("@unFechaReproduccion", reproduccion.FechaReproduccion);
        await _conexion.ExecuteAsync("altaHistorial_reproduccion", parametros, commandType: CommandType.StoredProcedure);

        reproduccion.IdHistorial = parametros.Get<uint>("@unidHistorial");
        return reproduccion;
    }

    public async Task<Reproduccion?> DetalleDeAsync(uint idHistorial)
    {
        var BuscarReproduccionPorId = @"SELECT * FROM HistorialReproduccion WHERE idHistorial = @idHistorial";
        var Buscar = await _conexion.QueryFirstOrDefaultAsync<Reproduccion>(BuscarReproduccionPorId, new {idHistorial});
        return Buscar;
    }

    public async Task<List<Reproduccion>> Obtener()
    {
        var task = await EjecutarSPConReturnDeTipoListaAsync<Reproduccion>("ObtenerHistorialReproduccion");
        return task.ToList();
    }
}