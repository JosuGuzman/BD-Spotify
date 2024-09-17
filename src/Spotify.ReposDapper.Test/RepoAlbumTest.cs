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

}
