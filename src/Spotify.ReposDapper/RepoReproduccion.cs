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
        parametros.Add("@uniCancion", reproduccion.cancion.idCancion);
        parametros.Add("@unFechaReproduccion", reproduccion.FechaReproccion);
        _conexion.Execute("altaHistorial_reproduccion", parametros, commandType: CommandType.StoredProcedure);

        reproduccion.IdHistorial = parametros.Get<uint>("@unidHistorial");

        return reproduccion.IdHistorial;
    }


    public IList<Reproduccion> Obtener()
    {
        string consultarReproducciones = @"SELECT * from HistorialReproducción ORDER BY FechaReproduccion ASC";
        var Reproducciones = _conexion.Query<Reproduccion>(consultarReproducciones);
        return Reproducciones.ToList();
    }
}
