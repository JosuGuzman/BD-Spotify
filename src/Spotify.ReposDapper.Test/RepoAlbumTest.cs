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
 
    [Theory]
    [InlineData("Digimon")]
    public void AltaAlbum(string titulo)
    {
        // Obtener un artista para la prueba
        var artis = _repoArtista.Obtener().First();

        // Crear un nuevo álbum
        var nuevoAlbum = new Album { Titulo = titulo, FechaLanzamiento = DateTime.Now, artista = artis};

        // Verifica si el título es null
        if (titulo == null)
        {
            Assert.Throws<InvalidOperationException>(() => _repoAlbum.Alta(nuevoAlbum));
            return; // Salir del método si el título es null
        }

        // Verifica si el álbum ya existe
        var albumExistente = _repoAlbum.Obtener().FirstOrDefault(a => a.Titulo == nuevoAlbum.Titulo);
        if (albumExistente != null)
        {
            Assert.Throws<InvalidOperationException>(() => _repoAlbum.Alta(nuevoAlbum));
            return; // Salir del método si el álbum ya existe
        }

        // Agrega el nuevo álbum
        var IdAlbum = _repoAlbum.Alta(nuevoAlbum);

        // Verifica que el álbum se haya añadido correctamente
        var albunes = _repoAlbum.Obtener();
        Assert.Contains(albunes, a => a.Titulo == "Baladas del Mar");
    }


    [Fact]
    public void EliminarAlbum()
    {

        // Obtener un álbum existente de la base de datos
        var EliminacionAlbum = _repoAlbum.Obtener().First();
        // Asegurandonos de que el álbum que vamos a eliminar existe
        Assert.NotNull(EliminacionAlbum);

        // Eliminar el álbum
        _repoAlbum.Eliminar(EliminacionAlbum.idAlbum);

        // Verificar que el álbum ya no está presente
        var albumesDespues = _repoAlbum.Obtener();
        Assert.DoesNotContain(albumesDespues, a => a.idAlbum == EliminacionAlbum.idAlbum);
    }   

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void DetalleIdAlbum(uint parametros)
    {
        
    }
}   