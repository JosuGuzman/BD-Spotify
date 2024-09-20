using Spotify.Core;
using Spotify.Core.Persistencia;
namespace Spotify.ReposDapper.Test;

public class RepoArtistaTest : TestBase
{
    private RepoArtista _repoArtista;
    public RepoArtistaTest() : base()
    {
        this._repoArtista = new RepoArtista(Conexion);
    }

    [Fact]
    public void ListarOk()
    {
        var ListaArtistas = _repoArtista.Obtener();

        Assert.NotNull(ListaArtistas);
    }

    [Fact]
    public void Alta()
    {
        var ArtistaInsertar = new Artista
        {
            NombreArtistico = "MARIO BUSTAMAN",
            Nombre = "Mario",
            Apellido = "Rojas Villalva"
        };

        var idArtistaInsertado = _repoArtista.Alta(ArtistaInsertar);

        var ListaArtistas = _repoArtista.Obtener();

        Assert.Contains(ListaArtistas, variable => variable.Nombre == "Mario");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]

    public void DetalleArtistaPorId(uint parametro)
    {
        var ArtistaPorId = _repoArtista.DetalleDe(parametro);

        Assert.NotNull(ArtistaPorId);
    }
}
