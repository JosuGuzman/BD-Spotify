using Spotify.Core;
namespace Spotify.ReposDapper.Test;

public class RepoAlbumTest : TestBase
{
    private RepoAlbum _repoAlbum;
    private RepoArtista _repoArtista;

    public RepoAlbumTest() : base()
    {
        _repoAlbum = new RepoAlbum(Conexion);
        _repoArtista = new RepoArtista(Conexion);

    } 

    [Fact]
    public void ListarOK()
    {
        var album = _repoAlbum.Obtener();
        Assert.Contains(album, a => a.Titulo == "Ecos del Pasado");
    }
 
        [Fact]
    public void AltaAlbum()
    {   
        var artis = _repoArtista.Obtener().First(); // Obtiene un artista para la prueba
        var nuevoAlbum = new Album { Titulo = "Baladas del Mar", FechaLanzamiento = DateTime.Now, artista = artis };
        var IdAlbum = _repoAlbum.Alta(nuevoAlbum);
    
        var Albunes = _repoAlbum.Obtener();
        Assert.Contains(Albunes, a => a.Titulo == "Baladas del Mar");
    }

}