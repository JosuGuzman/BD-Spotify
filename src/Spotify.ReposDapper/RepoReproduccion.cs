namespace Spotify.ReposDapper;

public class RepoReproduccion : RepoGenerico, IRepoReproduccion
{
    public RepoReproduccion(IDbConnection conexion) 
        : base(conexion) {}

    public void Alta(Reproduccion reproduccion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidHistorial", direction: ParameterDirection.Output);
        parametros.Add("@unidUsuario", reproduccion.usuario.IdUsuario);
        parametros.Add("@uniCancion", reproduccion.Canciones.IdCancion);
        parametros.Add("@unFechaReproduccion", reproduccion.FechaReproccion);
        _conexion.Execute("altaHistorial_reproduccion", parametros, commandType: CommandType.StoredProcedure);

        reproduccion.IdHistorial = parametros.Get<int>("@unidHistorial");
    }
 
    public IList<Reproduccion> Obtener()
    {
        string consultarReproducciones = @"SELECT * from HistorialReproducción ORDER BY FechaReproduccion ASC";
        var Reproducciones = _conexion.Query<Reproduccion>(consultarReproducciones);
        return Reproducciones.ToList();
    }
}
