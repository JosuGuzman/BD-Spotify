namespace Spotify.ReposDapper;

public class RepoSuscripcion : RepoGenerico, IRepoRegistro
{
    public RepoSuscripcion(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(Registro registro)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidSuscripcion", direction : ParameterDirection.Output);
        parametros.Add("@unidUsuario",registro.usuario.IdUsuario);
        parametros.Add("@unidTipoSuscripcion", registro.tipoSuscripcion.IdTipoSuscripcion);
        parametros.Add("@unFechaInicio", registro.FechaInicio);
        
        
        _conexion.Execute("altaRegistroSuscripcion", parametros, commandType: CommandType.StoredProcedure);

        registro.idSuscripcion = parametros.Get<uint>("@unidSuscripcion");
        return registro.idSuscripcion;
    }
    

    public IList<Registro> Obtener()
    {
        string consultarRegistros = @"SELECT * from Suscripcion ORDER BY FechaInicio ASC";
        var Registros = _conexion.Query<Registro>(consultarRegistros);
        return Registros.ToList();
    }
}