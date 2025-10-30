namespace Spotify.ReposDapper;

public class RepoGeneroAsync : RepoGenerico, IRepoGeneroAsync
{
    public RepoGeneroAsync(IDbConnection conexion)
        : base(conexion) {}

    public async Task<Genero> AltaAsync(Genero genero)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unGenero", genero.genero);
        parametros.Add("@unDescripcion", genero.Descripcion);
        parametros.Add("@unidGenero", direction: ParameterDirection.Output);

        await _conexion.ExecuteAsync("altaGenero", parametros, commandType: CommandType.StoredProcedure);
        genero.idGenero = parametros.Get<byte>("@unidGenero");
        return genero;
    }

    public async Task<Genero?> DetalleDeAsync(byte idGenero)
    {
        var BuscarGeneroPorId = @"SELECT * FROM Genero WHERE idGenero = @idGenero";
        var Buscar = await _conexion.QueryFirstOrDefaultAsync<Genero>(BuscarGeneroPorId, new { idGenero });
        return Buscar;
    }

    public async Task EliminarAsync(byte idGenero)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidGenero", idGenero);
        await _conexion.ExecuteAsync("eliminarGenero", parametros, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<Genero>> Obtener()
    { 
        var task = await EjecutarSPConReturnDeTipoListaAsync<Genero>("ObtenerGeneros");
        return task.ToList();
    }
}