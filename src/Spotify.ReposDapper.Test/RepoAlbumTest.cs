using Spotify.Core;
using Spotify.Core.Persistencia;
namespace Spotify.ReposDapper.Test;

public class RepoAlbumTest : TestBase
{
    IRepoAlbum _repo;
    
    public RepoAlbumTest() : base()
        => _repo = new RepoAlbum(Conexion);

    [Fact]
    public void ListarOK()
    {
        var album = _repo.Obtener();
        Assert.Contains(album, a => a.Titulo == "Ecos del Pasado");
    }

    [Fact]
    public void AltaAlbum()
    {
        var nuevoAlbum = new Album { Titulo = "Rock", FechaLanzamiento == '2024-09-12', artista = "mIGUEL" };
        var IdAlbum = _repo.Alta(nuevoAlbum);
        
        var Albunes = _repo.Obtener();
        Assert.Contains(Albunes, );
    }

}
