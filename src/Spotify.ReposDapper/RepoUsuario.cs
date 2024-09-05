namespace Spotify.ReposDapper;

public class RepoUsuario : RepoGenerico, IRepoUsuario
{
    public RepoUsuario(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(Usuario usuario)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidUsuario", direction: ParameterDirection.Output);
        parametros.Add("@unNombreUsuario", usuario.NombreUsuario);
        parametros.Add("@unaContraseña", usuario.Contraseña);
        parametros.Add("@unEmail", usuario.Gmail);
        parametros.Add("@unidNacionalidad", usuario.nacionalidad.IdNacionalidad);

        _conexion.Execute("altaTipoSuscripcion", parametros, commandType: CommandType.StoredProcedure);

        usuario.IdUsuario = parametros.Get<uint>("@unidUsuario");
        return usuario.IdUsuario;
    }


    public IList<Usuario> Obtener()
    {
        string consultarUsuarios = @"SELECT * from Usuario ORDER BY NombreUsuario ASC";
        var Usuarios = _conexion.Query<Usuario>(consultarUsuarios);
        return Usuarios.ToList();
    }
}
 