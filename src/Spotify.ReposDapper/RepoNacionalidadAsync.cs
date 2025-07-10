namespace Spotify.ReposDapper;

public class RepoNacionalidadAsync : RepoGenerico, IRepoNacionalidadAsync
{
    public RepoNacionalidadAsync(IDbConnection conexion)
    : base(conexion) {}

    public async Task<Nacionalidad> AltaAsync(Nacionalidad nacionalidad)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidNacionalidad", direction: ParameterDirection.Output);
        parametros.Add("@unPais", nacionalidad.Pais);

        await _conexion.ExecuteAsync("altaNacionalidad", parametros, commandType: CommandType.StoredProcedure);

        nacionalidad.idNacionalidad = parametros.Get<uint>("@unidNacionalidad");

        return nacionalidad;
    }

    public async Task<Nacionalidad?> DetalleDeAsync(uint idNacionalidad)
    {
        var BuscarNacionalidadPorId = @"SELECT * FROM Nacionalidad WHERE idNacionalidad = @idNacionalidad";

        var Buscar = await _conexion.QueryFirstOrDefaultAsync<Nacionalidad>(BuscarNacionalidadPorId, new { idNacionalidad });

        return Buscar;
    }

    public IList<Nacionalidad> Obtener () => EjecutarSPConReturnDeTipoLista<Nacionalidad>("ObtenerNacionalidades").ToList();
}