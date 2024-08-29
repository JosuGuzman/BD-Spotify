namespace Spotify.ReposDapper;

public class RepoUsuario : RepoGenerico, IRepoUsuario
{
    public RepoUsuario(IDbConnection conexion) 
        : base(conexion) {}

    public void Alta(Usuario usuario)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidUsuario", direction: ParameterDirection.Output);
        parametros.Add("@unNombreUsuario", usuario.NombreUsuario);
        parametros.Add("@unaContraseña", usuario.Contraseña);
        parametros.Add("@unEmail", usuario.Gmail);
        parametros.Add("@unidNacionalidad", usuario.nacionalidad.IdNacionalidad);

        _conexion.Execute("altaTipoSuscripcion", parametros, commandType: CommandType.StoredProcedure);

        usuario.IdUsuario = parametros.Get<uint>("@unidUsuario");
    }


    public IList<Usuario> Obtener()
    {
        ();
    }
}
 