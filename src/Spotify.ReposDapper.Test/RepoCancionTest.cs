using Spotify.Core;
using Spotify.Core.Persistencia;
namespace Spotify.ReposDapper.Test;

public class RepoCancionTest : TestBase
{
    private RepoAlbum _repoAlbum;
    private RepoGenero _repoGenero;
    private RepoArtista _repoArtista;
    private RepoCancion _repoCancion;
    public RepoCancionTest() : base()
    {
        this._repoCancion = new RepoCancion(Conexion);
        this._repoArtista = new RepoArtista(Conexion);
        this._repoAlbum = new RepoAlbum(Conexion);
        this._repoGenero = new RepoGenero(Conexion);
    }

    [Fact]
    public void ListarOk()
    {
        var ListaCanciones = _repoCancion.Obtener();

        Assert.NotNull(ListaCanciones);
    }

    [Fact]
    public void AltaCancion()
    {
        var unArtista = _repoArtista.Obtener().First();
        var unAlbum = _repoAlbum.Obtener().First();
        var unGenero = _repoGenero.Obtener().First();
        
        var InsertarCancion = new Cancion 
        {
            Titulo = "Lamento ser boliviano",
            Duracion = new TimeSpan(0,15,2),
            album = unAlbum,
            genero = unGenero,
            artista = unArtista
        };

        var idCancionInsertada = _repoCancion.Alta(InsertarCancion);

        var ListaCanciones = _repoCancion.Obtener();

        Assert.Contains(ListaCanciones, variable => variable.idCancion == idCancionInsertada);

    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]

    public void DetallaIdCanciones(uint idCancion)
    {
        var BuscarIdCanciones = _repoCancion.DetalleDe(idCancion);

        Assert.NotNull(BuscarIdCanciones);
        Assert.Equal(idCancion, BuscarIdCanciones.idCancion);
    }
}
