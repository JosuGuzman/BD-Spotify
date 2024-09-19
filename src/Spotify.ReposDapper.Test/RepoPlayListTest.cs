using Spotify.Core;
using Spotify.Core.Persistencia;
namespace Spotify.ReposDapper.Test;

public class RepoPlayListTest : TestBase
{
    IRepoPlaylist _repo;
    
    public RepoPlayListTest() : base()
        => _repo = new RepoPlaylist(Conexion);


    [Fact]
    public void ListarOK()
    {
        var playLists = _repo.Obtener();
        Assert.Contains(playLists, p => p.idPlaylist == 5);
    }

    [Fact]
    public void AltaPlaylistOk()
    {
        
    }
}
