namespace Spotify.ReposDapper;

public class RepoTipoSuscripcionAsync : RepoGenerico, IRepoTipoSuscripcionAsync
{
    public RepoTipoSuscripcionAsync(IDbConnection conexion) 
        : base(conexion) {}

    public async Task<TipoSuscripcion> AltaAsync(TipoSuscripcion tipoSuscripcion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidTipoSuscripcion", direction: ParameterDirection.Output);
        parametros.Add("@unCosto", tipoSuscripcion.Costo);
        parametros.Add("@unaDuracion", tipoSuscripcion.Duracion);
        parametros.Add("@UntipoSuscripcion", tipoSuscripcion.Tipo);

        await _conexion.ExecuteAsync("altaTipoSuscripcion", parametros, commandType: CommandType.StoredProcedure);
        tipoSuscripcion.IdTipoSuscripcion = parametros.Get<uint>("@unidTipoSuscripcion");
        return tipoSuscripcion;
    }

    public async Task<TipoSuscripcion?> DetalleDeAsync(uint idTipoSuscripcion)
    {
        var BuscarTipoSuscripcionPorId = @"
        Select * 
        FROM TipoSuscripcion
        Where idTipoSuscripcion = @idTipoSuscripcion
        ";
        
        var TipoSuscripcion = await _conexion.QueryFirstOrDefaultAsync<TipoSuscripcion>(BuscarTipoSuscripcionPorId, new {idTipoSuscripcion});
        return TipoSuscripcion;
    }

    public async Task<List<TipoSuscripcion>> Obtener()
    {
        var task = await EjecutarSPConReturnDeTipoListaAsync<TipoSuscripcion>("ObtenerTipoSuscripciones");
        return task.ToList();
    }
}