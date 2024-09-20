using Xunit;

namespace Spotify.ReposDapper.Test;

public class RepoCancionTest : TestBase
{
    private RepoAlbum _repoAlbum;
    private RepoGenero _repoGenero;
    private RepoCancion _repoCancion;
    public RepoCancionTest() : base()
    {
        this._repoCancion = new RepoCancion(Conexion);
        this._repoAlbum = new RepoAlbum(Conexion);
        this._repoGenero = new RepoGenero(Conexion);
    }

    [Fact]
    public void AltaCancion()
    {
        var unAlbum = _repoAlbum.Obtener().First();
        var unGenero = _repoGenero.Obtener().First();
        
        var InsertarCancion = new Cancion 
        {
            Titulo = "Lamento boliviano",
            Duracion = new TimeSpan(0,15,2),
            album = unAlbum,
            genero = unGenero
        };

        var idCancionInsertada = _repoCancion.Alta(InsertarCancion);

        var ListaCanciones = _repoCancion.Obtener();

        Assert.Contains(ListaCanciones, variable => variable.idCancion == idCancionInsertada);

    }
}
