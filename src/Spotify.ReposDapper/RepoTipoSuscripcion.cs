namespace Spotify.ReposDapper;

public class RepoTipoSuscripcion : RepoGenerico, IRepoTipoSuscripcion
{
    public RepoTipoSuscripcion(IDbConnection conexion) 
        : base(conexion) {}

    public void Alta(TipoSuscripcion tipoSuscripcion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidCancion", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", cancion.Titulo);
        parametros.Add("@unDuration", cancion.Duracion);

        _conexion.Execute("altaCancion", parametros, commandType: CommandType.StoredProcedure);

        cancion.IdCancion = parametros.Get<uint>("@unidCancion");
    }

    public IList<TipoSuscripcion> Obtener()
    { 
        throw new NotImplementedException();
    }
}
 