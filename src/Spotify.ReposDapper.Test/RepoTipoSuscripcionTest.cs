using Xunit;

namespace Spotify.ReposDapper.Test;

public class RepoTipoSuscripcionTest : TestBase
{
    private RepoTipoSuscripcion _repoTipoSuscripcion;

    public RepoTipoSuscripcionTest() : base()
    {
        this._repoTipoSuscripcion = new RepoTipoSuscripcion(Conexion);
    }

    [Fact]
    public void ListarOk()
    {
        var ListaTiposSuscripcion = _repoTipoSuscripcion.Obtener();

        Assert.NotEmpty(ListaTiposSuscripcion);
    }

    [Fact]
    public void AltaTipoSuscripcion()
    {

    }
}
