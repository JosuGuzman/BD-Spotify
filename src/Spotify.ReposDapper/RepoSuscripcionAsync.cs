namespace Spotify.ReposDapper;

public class RepoSuscripcionAsync : RepoGenerico, IRepoRegistroAsync
{
    public RepoSuscripcionAsync(IDbConnection conexion) 
        : base(conexion) { }

    public Task<Registro> AltaAsync(Registro registro)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidSuscripcion", direction: ParameterDirection.Output);
        parametros.Add("@unidUsuario", registro.usuario.idUsuario);
        parametros.Add("@unidTipoSuscripcion", registro.tipoSuscripcion.IdTipoSuscripcion);
        parametros.Add("@unFechaInicio", registro.FechaInicio);
        
        await _conexion.ExecuteAsync("altaRegistroSuscripcion", parametros, commandType: CommandType.StoredProcedure);
        registro.idSuscripcion = parametros.Get<uint>("@unidSuscripcion");
        return registro.idSuscripcion;
    }

    public Task<Registro?> DetalleDeAsync(uint id)
    {
        var sql = @"SELECT * FROM Suscripcion WHERE idSuscripcion = @idSuscripcion";
        return await _conexion.QueryFirstOrDefaultAsync<Registro>(sql, new { idSuscripcion });
    }

    public Task<List<Registro>> Obtener()
    {
        var resultados = await EjecutarSPConReturnDeTipoListaAsync<Registro>("ObtenerSuscripciones");
        return resultados.ToList();
    }
}