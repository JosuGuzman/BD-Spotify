namespace Spotify.ReposDapper;

public class RepoNacionalidad : RepoGenerico, IRepoNacionalidad
{
    public RepoNacionalidad(IDbConnection conexion) 
        : base(conexion) {}
    
    public uint Alta (Nacionalidad nacionalidad )
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidNacionalidad", direction: ParameterDirection.Output);
        parametros.Add("@unPais", nacionalidad.Pais);

        _conexion.Execute("altaNacionalidad", parametros, commandType: CommandType.StoredProcedure);

        nacionalidad.idNacionalidad = parametros.Get<uint>("@unidNacionalidad");

        return nacionalidad.idNacionalidad;
    }

    public Nacionalidad? DetalleDe(uint idNacionalidad)
    {
        var BuscarNacionalidadPorId = @"SELECT * FROM Nacionalidad WHERE idNacionalidad = @idNacionalidad";

        var Buscar = _conexion.QueryFirstOrDefault<Nacionalidad>(BuscarNacionalidadPorId, new{idNacionalidad});

        return Buscar;
    }

    public async Task<Nacionalidad?> DetalleDeAsync(uint idNacionalidad)
    {
        var BuscarNacionalidadPorId = @"SELECT * FROM Nacionalidad WHERE idNacionalidad = @idNacionalidad";

        var Buscar = await _conexion.QueryFirstOrDefaultAsync<Nacionalidad>(BuscarNacionalidadPorId, new { idNacionalidad });

        return Buscar;
    }

    public IList<Nacionalidad> Obtener () => EjecutarSPConReturnDeTipoLista<Nacionalidad>("ObtenerNacionalidades").ToList();
}