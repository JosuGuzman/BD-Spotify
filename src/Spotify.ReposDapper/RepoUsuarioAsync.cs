
namespace Spotify.ReposDapper;

public class RepoUsuarioAsync : RepoGenerico, IRepoUsuarioAsync
{
    public RepoUsuarioAsync(IDbConnection conexion) 
        : base(conexion) {}

    public async Task<Usuario> AltaAsync(Usuario usuario)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidUsuario", direction: ParameterDirection.Output);
        parametros.Add("@unNombreUsuario", usuario.NombreUsuario);
        parametros.Add("@unaContrasenia", usuario.Contrasenia);
        parametros.Add("@unEmail", usuario.Gmail);
        parametros.Add("@unidNacionalidad", usuario.nacionalidad.idNacionalidad);

        await _conexion.ExecuteAsync("altaUsuario", parametros, commandType: CommandType.StoredProcedure);

        usuario.idUsuario = parametros.Get<uint>("@unidUsuario");
        return usuario;
    }

    public async Task<Usuario?> DetalleDeAsync(uint idUsuario)
    {
        string BuscarUsuario = @"SELECT * FROM Usuario WHERE idUsuario = @idUsuario";

        var usuario = await _conexion.QueryFirstOrDefaultAsync<Usuario>(BuscarUsuario, new { idUsuario });

        return usuario;
    }

    public IList<Usuario> Obtener() => EjecutarSPConReturnDeTipoLista<Usuario>("ObtenerUsuarios").ToList();
}