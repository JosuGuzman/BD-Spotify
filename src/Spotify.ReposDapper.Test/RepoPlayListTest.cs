using Spotify.Core;
using Spotify.Core.Persistencia;
namespace Spotify.ReposDapper.Test;

public class RepoPlayListTest : TestBase
{
    RepoPlaylist _repoPlayList;
    RepoUsuario _repoUsuario;
    RepoCancion _repoCancion;
    
    public RepoPlayListTest() : base()
    {
        _repoPlayList = new RepoPlaylist(Conexion);
        _repoUsuario = new RepoUsuario(Conexion);
        _repoCancion = new RepoCancion(Conexion);
    } 


    [Fact]
    public void ListarOK()
    {
        var playLists = _repoPlayList.Obtener();
        Assert.Contains(playLists, p => p.idPlaylist == 5);
    }

    [Fact]
    public void AltaPlaylistOk()
    {
        var MuchasCanciones = _repoCancion.Obtener().ToList();
        var unUsuario = _repoUsuario.Obtener().First(); 

        var Playlist_a_insertar = new PlayList 
        {
            Nombre = "Tus labios me tocan",
            usuario = unUsuario,
            Canciones = MuchasCanciones
        };

        var idPlayListAdarAlta = _repoPlayList.Alta(Playlist_a_insertar);

        var ListaPlaylists = _repoPlayList.Obtener();

        Assert.Contains(ListaPlaylists, variable => variable.idPlaylist == idPlayListAdarAlta);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]

    public void DetalleIdPlaylist(uint parametro)
    {
        var PlayListPorId = _repoPlayList.DetalleDe(parametro);

        Assert.NotNull(PlayListPorId);
    }
}
