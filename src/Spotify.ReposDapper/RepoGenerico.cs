namespace Spotify.ReposDapper;
public abstract class RepoGenerico
{
    protected readonly IDbConnection _conexion;
    protected RepoGenerico(IDbConnection conexion) => _conexion = conexion;
}
   