
namespace Spotify.ReposDapper;

public class RepoReproduccion : RepoGenerico, IRepoReproduccion
{
    public RepoReproduccion(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(Reproduccion reproduccion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidHistorial", direction: ParameterDirection.Output);
        parametros.Add("@unidUsuario", reproduccion.usuario.idUsuario);
        parametros.Add("@unidCancion", reproduccion.cancion.idCancion);
        parametros.Add("@unFechaReproduccion", reproduccion.FechaReproduccion);
        _conexion.Execute("altaHistorial_reproduccion", parametros, commandType: CommandType.StoredProcedure);

        reproduccion.IdHistorial = parametros.Get<uint>("@unidHistorial");

        return reproduccion.IdHistorial;
    }

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

    public Reproduccion? DetalleDe(uint idHistorial)
    {
        var BuscarReproduccionPorId = @"SELECT * FROM HistorialReproduccion WHERE idHistorial = @idHistorial";

        var Buscar = _conexion.QueryFirstOrDefault<Reproduccion>(BuscarReproduccionPorId, new {idHistorial});
        
        return Buscar;
    }

    public async Task<Reproduccion?> DetalleDeAsync(uint idHistorial)
    {
        var BuscarReproduccionPorId = @"SELECT * FROM HistorialReproduccion WHERE idHistorial = @idHistorial";

        var Buscar = await _conexion.QueryFirstOrDefaultAsync<Reproduccion>(BuscarReproduccionPorId, new { idHistorial });

        return Buscar;
    }

    public Task EliminarAsync(uint id)
    {
        throw new NotImplementedException();
    }

    public IList<Reproduccion> Obtener() => EjecutarSPConReturnDeTipoLista<Reproduccion>("ObtenerHistorialReproduccion").ToList();

    public Task<IEnumerable<Reproduccion>> ObtenerAsync()
    {
        throw new NotImplementedException();
    }
}
