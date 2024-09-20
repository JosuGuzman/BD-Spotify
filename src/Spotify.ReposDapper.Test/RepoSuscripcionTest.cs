using Spotify.Core;
using Spotify.Core.Persistencia;
namespace Spotify.ReposDapper.Test;

public class RepoSuscripcionTest : TestBase
{
    private RepoUsuario _repoUsuario;
    private RepoTipoSuscripcion _repoTipoSuscripcion;
    private RepoSuscripcion _repoSuscripcion;
    public RepoSuscripcionTest() : base()
    {
        this._repoSuscripcion = new RepoSuscripcion(Conexion);
        this._repoTipoSuscripcion = new RepoTipoSuscripcion(Conexion);
        this._repoUsuario = new RepoUsuario(Conexion);
    }

    [Fact]
    public void ListarOk()
    {
        var ListaSuscripciones = _repoSuscripcion.Obtener();

        Assert.NotNull(ListaSuscripciones);
    }

    [Fact]
    public void AltaSuscripcion()
    {
        var unUsuario = _repoUsuario.Obtener().First();
        var unTipoSuscripcion = _repoTipoSuscripcion.Obtener().First();

        var suscripcion = new Registro{
            usuario = unUsuario,
            tipoSuscripcion = unTipoSuscripcion,
            FechaInicio = DateTime.Now
        };

        var idSuscripcionCreada = _repoSuscripcion.Alta(suscripcion);
        var ListaRegistroDeSuscripciones = _repoSuscripcion.Obtener();

        Assert.Contains(ListaRegistroDeSuscripciones, variable => variable.idSuscripcion == idSuscripcionCreada);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]

    public void DetallePorIdDeSuscripcion(uint parametro)
    {
        var SuscripcionPorId = _repoSuscripcion.DetalleDe(parametro);

        Assert.NotNull(SuscripcionPorId);
    }
}
