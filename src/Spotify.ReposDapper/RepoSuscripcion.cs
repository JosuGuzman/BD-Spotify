namespace Spotify.ReposDapper;

public class RepoSuscripcion : RepoGenerico, IRepoRegistro
{
    public RepoSuscripcion(IDbConnection conexion) 
        : base(conexion) {}

    public void Alta(Registro registro)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidUsuario",registro.Usuarios.IdUsuario);
        parametros.Add("@unidTipoSuscripcion", registro.Suscripciones.IdSuscripcion);
        parametros.Add("@unFechaInicio", registro.FechaInicio);

    }

    public IList<Registro> Obtener()
    {
        string consultarRegistros = @"SELECT * from Suscripcion ORDER BY FechaInicio ASC";
        var Registros = _conexion.Query<Registro>(consultarRegistros);
        return Registros.ToList();
    }
}