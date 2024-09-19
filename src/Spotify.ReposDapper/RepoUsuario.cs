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
        parametros.Add("@unidNacionalidad", usuario.nacionalidad.idNacionalidad);

        _conexion.Execute("altaUsuario", parametros, commandType: CommandType.StoredProcedure);

        usuario.idUsuario = parametros.Get<uint>("@unidUsuario");
        return usuario.idUsuario;
    }

    public Usuario? DetalleDe(uint idUsuario)
    {
        string BuscarUsuario = @"SELECT NombreUsuario FROM Usuario WHERE idUsuario = @idUsuario";

        // Ejecutar la consulta y obtener el primer resultado o 'null' si no existe.
        var usuario = _conexion.QueryFirstOrDefault<Usuario>(
            BuscarUsuario, 
            new { idUsuario }
        );

        return usuario;
    }

    public void Eliminar(uint elemento)
    {
        throw new NotImplementedException();
    }

    public IList<Usuario> Obtener()
    {
        string consultarUsuarios = @"SELECT * from Usuario ORDER BY NombreUsuario ASC";
        var Usuarios = _conexion.Query<Usuario>(consultarUsuarios);
        return Usuarios.ToList();
    }
}
 