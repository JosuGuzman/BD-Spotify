namespace Spotify.ReposDapper;

public class RepoSuscripcion : RepoGenerico, IRepoSuscripcion
{
    public RepoSuscripcion(IDbConnection conexion) 
        : base(conexion) {}

    public void Alta(Suscripcion suscripcion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unid", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", suscripcion);
        parametros.Add("@unidArtista", suscripcion);

    }

    public IList<Suscripcion> Obtener()
    {
        gfhfghfgh
    }
}