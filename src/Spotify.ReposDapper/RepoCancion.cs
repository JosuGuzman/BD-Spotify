namespace Spotify.ReposDapper;
public class RepoCancion : RepoGenerico, IRepoCancion
{
    public RepoCancion(IDbConnection conexion)
        : base(conexion) { }

    public void Alta(Cancion cancion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidCancion", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", cancion.Titulo);
        parametros.Add("@unDuration", cancion.Duracion);

        _conexion.Execute("altaCancion", parametros, commandType: CommandType.StoredProcedure);

        cancion.IdCancion = parametros.Get<uint>("@unidCancion");
    } 
 
    public IList<Cancion> Obtener()
    {
        string consultarCanciones = @"SELECT * from Cancion ORDER BY Titulo ASC";
        var Canciones = _conexion.Query<Cancion>(consultarCanciones);
        return Canciones.ToList();
    }

}