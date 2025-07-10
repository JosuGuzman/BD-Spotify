
using System.Threading.Tasks;

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

    public async Task<Usuario?> DetalleDe(uint idUsuario)
    {
        string BuscarUsuario = @"SELECT * FROM Usuario WHERE idUsuario = @idUsuario";

        var usuario = await _conexion.QueryFirstOrDefaultAsync<Usuario>(BuscarUsuario, new { idUsuario });

        return usuario;
    }

    public async Task<Usuario?> DetalleDeAsync(uint id)
    {
        string BuscarUsuario = @"SELECT * FROM Usuario WHERE idUsuario = @idUsuario";

        var usuario = await _conexion.QueryFirstOrDefaultAsync<Usuario>(BuscarUsuario, new { idUsuario });

        return usuario;
    }

    public void Eliminar(uint elemento)
    {
        throw new NotImplementedException();
    }

    public Task EliminarAsync(uint id)
    {
        throw new NotImplementedException();
    }

    public IList<Usuario> Obtener() => EjecutarSPConReturnDeTipoLista<Usuario>("ObtenerUsuarios").ToList();

    public Task<IEnumerable<Usuario>> ObtenerAsync()
    {
        throw new NotImplementedException();
    }
}