
namespace Spotify.ReposDapper;

public class RepoTipoSuscripcion : RepoGenerico, IRepoTipoSuscripcion
{
    public RepoTipoSuscripcion(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(TipoSuscripcion tipoSuscripcion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidTipoSuscripcion", direction: ParameterDirection.Output);
        parametros.Add("@unCosto", tipoSuscripcion.Costo);
        parametros.Add("@unaDuracion", tipoSuscripcion.Duracion);
        parametros.Add("@UntipoSuscripcion", tipoSuscripcion.Tipo);

        _conexion.Execute("altaTipoSuscripcion", parametros, commandType: CommandType.StoredProcedure);

        tipoSuscripcion.IdTipoSuscripcion = parametros.Get<uint>("@unidTipoSuscripcion");

        return tipoSuscripcion.IdTipoSuscripcion;
    }

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

    public TipoSuscripcion DetalleDe(uint idTipoSuscripcion)
    {
        var BuscarTipoSuscripcionPorId = @"
        Select * 
        FROM TipoSuscripcion
        Where idTipoSuscripcion = @idTipoSuscripcion
        ";
        
        var TipoSuscripcion = _conexion.QueryFirstOrDefault<TipoSuscripcion>(BuscarTipoSuscripcionPorId, new {idTipoSuscripcion});

        return TipoSuscripcion;
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

    public Task EliminarAsync(uint id)
    {
        throw new NotImplementedException();
    }

    public IList<TipoSuscripcion> Obtener() => EjecutarSPConReturnDeTipoLista<TipoSuscripcion>("ObtenerTipoSuscripciones").ToList();

    public Task<IEnumerable<TipoSuscripcion>> ObtenerAsync()
    {
        throw new NotImplementedException();
    }
}