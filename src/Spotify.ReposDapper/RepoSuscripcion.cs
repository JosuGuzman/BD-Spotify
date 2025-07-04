namespace Spotify.ReposDapper;

public class RepoSuscripcion : RepoGenerico, IRepoRegistro
{
    public RepoSuscripcion(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(Registro registro)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidSuscripcion", direction : ParameterDirection.Output);
        parametros.Add("@unidUsuario",registro.usuario.idUsuario);
        parametros.Add("@unidTipoSuscripcion", registro.tipoSuscripcion.IdTipoSuscripcion);
        parametros.Add("@unFechaInicio", registro.FechaInicio);
        
        
        _conexion.Execute("altaRegistroSuscripcion", parametros, commandType: CommandType.StoredProcedure);

        registro.idSuscripcion = parametros.Get<uint>("@unidSuscripcion");
        return registro.idSuscripcion;
    }

    public async Task<Registro> AltaAsync(Registro registro)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidSuscripcion", direction : ParameterDirection.Output);
        parametros.Add("@unidUsuario",registro.usuario.idUsuario);
        parametros.Add("@unidTipoSuscripcion", registro.tipoSuscripcion.IdTipoSuscripcion);
        parametros.Add("@unFechaInicio", registro.FechaInicio);
        
        
        await _conexion.ExecuteAsync("altaRegistroSuscripcion", parametros, commandType: CommandType.StoredProcedure);

        registro.idSuscripcion = parametros.Get<uint>("@unidSuscripcion");
        return registro;
    }

    public Registro DetalleDe(uint idSuscripcion)
    {
        var BuscarPorIdRegistro = @"SELECT * FROM Suscripcion Where idSuscripcion = @idSuscripcion";

        var Registro = _conexion.QueryFirstOrDefault<Registro>(BuscarPorIdRegistro, new {idSuscripcion});

        return Registro;
    }

    public async Task<Registro?> DetalleDeAsync(uint idSuscripcion)
    {
        var BuscarPorIdRegistro = @"SELECT * FROM Suscripcion Where idSuscripcion = @idSuscripcion";

        var Registro = await _conexion.QueryFirstOrDefaultAsync<Registro>(BuscarPorIdRegistro, new {idSuscripcion});

        return Registro;
    }

    public Task EliminarAsync(uint id)
    {
        throw new NotImplementedException();
    }

    public IList<Registro> Obtener() => EjecutarSPConReturnDeTipoLista<Registro>("ObtenerSuscripciones").ToList();

    public Task<IEnumerable<Registro>> ObtenerAsync()
    {
        throw new NotImplementedException();
    }
}