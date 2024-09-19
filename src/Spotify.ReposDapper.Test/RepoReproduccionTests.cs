
namespace Spotify.ReposDapper.Test;

public class RepoReproduccionTests : TestBase 
{
    private RepoUsuario _repoUsuario;
    private RepoCancion _repoCancion;
    private RepoReproduccion _repoReproduccion;

    public RepoReproduccionTests() : base()
    {
        this._repoReproduccion = new RepoReproduccion(Conexion);
    }

    [Fact]

    public void ListarOk()
    {
        var ListaDeReproducciones = _repoReproduccion.Obtener();

        Assert.NotNull(ListaDeReproducciones);
    }

    [Fact]
    public void AltaReproduccion()
    {
        var unUsuario = _repoUsuario.Obtener().First();
        var unaCancion = _repoCancion.Obtener().First();

        var CrearReproduccion = new Reproduccion
        {
            usuario = unUsuario,
            cancion = unaCancion,
            FechaReproccion = DateTime.Now
        };

        var idCreadoReproduccion = _repoReproduccion.Alta(CrearReproduccion);
        var ListadoDeReproducciones = _repoReproduccion.Obtener();

        Assert.Contains(ListadoDeReproducciones, variable => variable.IdHistorial == idCreadoReproduccion);
    }
}
