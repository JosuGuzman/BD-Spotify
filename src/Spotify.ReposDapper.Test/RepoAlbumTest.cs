using System.Diagnostics.CodeAnalysis;
using Spotify.Core;
using Spotify.Core.Persistencia;
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
        var nuevoAlbum = new Album { Titulo = "Rock", FechaLanzamiento = DateTime.Now, artista = _repoArtista.};
        var IdAlbum = _repoAlbum.Alta(nuevoAlbum);
        
        var Albunes = _repoAlbum.Obtener();
        Assert.Contains(Albunes, );
    }

}
