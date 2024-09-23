using Spotify.Core;
using Spotify.Core.Persistencia;
namespace Spotify.ReposDapper.Test;

public class RepoNacionalidadTest : TestBase
{
    RepoNacionalidad _repoNacionalidad;
    
    public RepoNacionalidadTest() : base()
    {
        _repoNacionalidad = new RepoNacionalidad(Conexion);
    }
        
    [Fact]
    public void ListarOK()
    {
        var nacionalidades = _repoNacionalidad.Obtener();

        Assert.Contains(nacionalidades, n=> n.Pais == "Argentina");
    }

    [Fact]
    public void AltaNacionalidadOK()
    {
        var nuevaNacionalidad = new Nacionalidad {Pais = "España"};
        var IdNAcionalidad = _repoNacionalidad.Alta(nuevaNacionalidad);

        var nacionalidades = _repoNacionalidad.Obtener();
        
        Assert.Contains(nacionalidades, n => n.Pais == "España");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]

    public void DetalleIdNacionalidad(uint idNacionalidad)
    {
        var BuscarNacionalidadPorId = _repoNacionalidad.DetalleDe(idNacionalidad);

        Assert.NotNull(BuscarNacionalidadPorId);
        Assert.Equal(idNacionalidad , BuscarNacionalidadPorId.idNacionalidad);
    }
}
