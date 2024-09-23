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
        Assert.NotNull(albunes);
    }
 

    [Fact]
    public void AltaAlbum()
    {
        var unArtista = _repoArtista.Obtener().First();
        // Obtener un artista para la prueba
        var nuevoAlbum = new Album { 
            Titulo = "Las 3 moscas",
            FechaLanzamiento = DateTime.Now,
            artista = unArtista
            };
        var idAlbumCreado = _repoAlbum.Alta(nuevoAlbum);
        
        var Albunes = _repoAlbum.Obtener();
        Assert.Contains(Albunes, a => a.idAlbum == idAlbumCreado);
    }


    /*[Fact]
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
    */
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void DetalleIdAlbum(uint idAlbum)
    {
        var AlbumPorId = _repoAlbum.DetalleDe(idAlbum);

        Assert.NotNull(AlbumPorId);
        Assert.Equal(idAlbum, AlbumPorId.idAlbum);
    }
}   