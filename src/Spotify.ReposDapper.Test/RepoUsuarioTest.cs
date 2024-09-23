using Spotify.Core;
using Spotify.Core.Persistencia;
namespace Spotify.ReposDapper.Test;

public class RepoUsuarioTest : TestBase
{
    private RepoUsuario _repoUsuario;
    private RepoNacionalidad _repoNacionalidad;

    public RepoUsuarioTest() : base()
    {
        this._repoUsuario = new RepoUsuario(Conexion);
        this._repoNacionalidad = new RepoNacionalidad(Conexion);
    }

    [Fact]
    public void ListarOk()
    {
        var listaUsuarios = _repoUsuario.Obtener();

        Assert.NotEmpty(listaUsuarios);
        Assert.NotNull(listaUsuarios);
    }

    [Fact]

    public void AltaUsuarioOk()
    {

        var unaNacionalidad = _repoNacionalidad.Obtener().First();

        var parametros = new Usuario 
        {
            NombreUsuario = "Sherklan12",
            Gmail = "elquequieraperdr@gmail.com",
            Contraseña = "RomanRiquelme",
            nacionalidad = unaNacionalidad
        };

        var idUsuarioCreado = _repoUsuario.Alta(parametros);
        var ListaUsuarios = _repoUsuario.Obtener();

        Assert.Contains(ListaUsuarios, variable => variable.idUsuario == idUsuarioCreado);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void DetalleIdUsuario (uint idUsuario)
    {
        var UsuarioPorId = _repoUsuario.DetalleDe(idUsuario);

        Assert.NotNull(UsuarioPorId);
        Assert.Equal(idUsuario, UsuarioPorId.idUsuario);
    }
}
