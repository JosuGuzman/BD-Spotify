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
        var albunes = _repoAlbum.Obtener();
        Assert.Contains(albunes, a => a.Titulo == "Ecos del Pasado");
    }
 
    [Fact]
    public void AltaAlbum()
    {   
        var artis = _repoArtista.Obtener().First(); // Obtiene un artista para la prueba
        var nuevoAlbum = new Album { Titulo = "Baladas del Mar", FechaLanzamiento = DateTime.Now, artista = artis };
        var IdAlbum = _repoAlbum.Alta(nuevoAlbum);
    
        var albunes = _repoAlbum.Obtener();
        Assert.Contains(albunes, a => a.Titulo == "Baladas del Mar");
    }

    [Fact]
    public void EliminarAlbum()
    {

        // Obtener un álbum existente de la base de datos
        var albumesAntes = _repoAlbum.Obtener();
        var albumAEliminar = albumesAntes.First(); // Suponiendo que hay al menos un álbum

        // Asegurandonos de que el álbum que vamos a eliminar existe
        Assert.NotNull(albumAEliminar);

        // Eliminar el álbum
        _repoAlbum.Eliminar(albumAEliminar.idAlbum);

        // Verificar que el álbum ya no está presente
        var albumesDespues = _repoAlbum.Obtener();
        Assert.DoesNotContain(albumesDespues, a => a.idAlbum == albumAEliminar.idAlbum);
    }   

}   