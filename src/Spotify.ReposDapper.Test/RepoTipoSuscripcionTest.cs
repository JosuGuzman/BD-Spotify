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
        Assert.NotNull(ListaTiposSuscripcion);
    }

    [Fact]
    public void AltaTipoSuscripcion()
    {
        var tipoSuscripcion = new TipoSuscripcion
        {
            Duracion = 4,
            Costo = 23,
            Tipo = "Anual"
        };

        var IdTipoSuscripcionCreado = _repoTipoSuscripcion.Alta(tipoSuscripcion);
        
        var ListaTiposSuscripcion = _repoTipoSuscripcion.Obtener();

        Assert.Contains(ListaTiposSuscripcion, variable => variable.IdTipoSuscripcion == IdTipoSuscripcionCreado);
    }
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void DetalleIdTipoSuscripcion(uint idTipoSuscripcion)
    {
        var tipoSuscripcionPorId = _repoTipoSuscripcion.DetalleDe(idTipoSuscripcion);

        Assert.NotNull(tipoSuscripcionPorId);
        Assert.Equal(idTipoSuscripcion, tipoSuscripcionPorId.IdTipoSuscripcion);
    }

    
}
