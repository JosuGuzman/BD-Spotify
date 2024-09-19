using Spotify.Core;
using Xunit;

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
        Assert.NotNull(listaUsuarios);
    }

    [Fact]

    public void AltaUsuarioOk()
    {
        var ListaUsuarios = _repoUsuario.Obtener();

        var unaNacionalidad = _repoNacionalidad.Obtener().First();

        var parametros = new Usuario 
        {
            NombreUsuario = "Sharklen12",
            Gmail = "elquequieraperdersutiempoquelopierda@gmail.com",
            Contraseña = "RomanRiquelme",
            nacionalidad = unaNacionalidad
        };

        var AltaUsuario = _repoUsuario.Alta(parametros);

        Assert.Contains(ListaUsuarios, variable => variable.NombreUsuario == "Sharklen12");
    }
}
