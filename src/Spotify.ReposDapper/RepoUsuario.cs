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
        parametros.Add("@unaContrasenia", usuario.Contrasenia);
        parametros.Add("@unEmail", usuario.Gmail);
        parametros.Add("@unidNacionalidad", usuario.nacionalidad.idNacionalidad);

        _conexion.Execute("altaUsuario", parametros, commandType: CommandType.StoredProcedure);

        usuario.idUsuario = parametros.Get<uint>("@unidUsuario");
        return usuario.idUsuario;
    }

    public Usuario? DetalleDe(uint idUsuario)
    {
        string BuscarUsuario = @"SELECT * FROM Usuario WHERE idUsuario = @idUsuario";
    
        var usuario = _conexion.QueryFirstOrDefault<Usuario>(BuscarUsuario, new { idUsuario });
        
        // Si encontramos el usuario, cargar la nacionalidad si existe
        if (usuario != null && usuario.nacionalidad?.idNacionalidad > 0)
        {
            string buscarNacionalidad = @"SELECT * FROM Nacionalidad WHERE idNacionalidad = @idNacionalidad";
            usuario.nacionalidad = _conexion.QueryFirstOrDefault<Nacionalidad>(
                buscarNacionalidad, 
                new { idNacionalidad = usuario.nacionalidad.idNacionalidad }
            );
        }
    
        return usuario;
    }

    public IList<Usuario> Obtener() => EjecutarSPConReturnDeTipoLista<Usuario>("ObtenerUsuarios").ToList();
}