namespace Spotify.ReposDapper;

public class RepoSuscripcion : RepoGenerico, IRepoRegistro
{
    public RepoSuscripcion(IDbConnection conexion) 
        : base(conexion) {}

    public void Alta(Registro registro)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidUsuario",registro.usuario.IdUsuario);
        parametros.Add("@unidTipoSuscripcion", registro.tipoSuscripcion.IdTipoSuscripcion);
        parametros.Add("@unFechaInicio", registro.FechaInicio);
        
        
        _conexion.Execute("altaRegistroSuscripcion", parametros, commandType: CommandType.StoredProcedure);


    }
    

    public IList<Registro> Obtener()
    {
        string consultarRegistros = @"SELECT * from Suscripcion ORDER BY FechaInicio ASC";
        var Registros = _conexion.Query<Registro>(consultarRegistros);
        return Registros.ToList();
    }
}