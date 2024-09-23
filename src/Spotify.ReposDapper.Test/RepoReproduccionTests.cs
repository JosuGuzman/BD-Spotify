namespace Spotify.ReposDapper.Test;

public class RepoReproduccionTests : TestBase 
{
    private RepoUsuario _repoUsuario;
    private RepoCancion _repoCancion;
    private RepoReproduccion _repoReproduccion;

    public RepoReproduccionTests() : base()
    {
        this._repoReproduccion = new RepoReproduccion(Conexion);
        this._repoUsuario = new RepoUsuario(Conexion);
        this._repoCancion = new RepoCancion (Conexion);
    }

    [Fact]

    public void ListarOk()
    {
        var ListaDeReproducciones = _repoReproduccion.Obtener();

        Assert.NotNull(ListaDeReproducciones);
        Assert.NotEmpty(ListaDeReproducciones);
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
            FechaReproduccion = DateTime.Now
        };

        var idCreadoReproduccion = _repoReproduccion.Alta(CrearReproduccion);
        var ListadoDeReproducciones = _repoReproduccion.Obtener();

        Assert.Contains(ListadoDeReproducciones, variable => variable.IdHistorial == idCreadoReproduccion);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]

    public void DetalleIdReproduccion(uint idReproduccion)
    {
        var ReproduccionPorId = _repoReproduccion.DetalleDe(idReproduccion);

        Assert.NotNull(ReproduccionPorId);
        Assert.Equal(idReproduccion, ReproduccionPorId.IdHistorial);
    }
}
