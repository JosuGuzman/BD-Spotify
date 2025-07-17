namespace Spotify.ReposDapper;

public abstract class RepoGenerico
{
    protected readonly IDbConnection _conexion;
    protected RepoGenerico(IDbConnection conexion) => _conexion = conexion;
    protected void EjecutarSPSinReturn(string nombreSP, DynamicParameters? parametros = null)
        => _conexion.Execute(nombreSP, param: parametros,
                            commandType: CommandType.StoredProcedure);

    public IEnumerable<T> EjecutarSPConReturnDeTipoLista<T>(string nombreSP, DynamicParameters? parametros = null)
        => _conexion.Query<T>(nombreSP, param: parametros, commandType: CommandType.StoredProcedure);

    public async Task<List<T>> EjecutarSPConReturnDeTipoListaAsync<T>(string nombreSP, DynamicParameters? parametros = null)
    {
    var task = await _conexion.QueryAsync<T>(nombreSP, param: parametros , commandType: CommandType.StoredProcedure);
    return task.ToList();
    }
}
