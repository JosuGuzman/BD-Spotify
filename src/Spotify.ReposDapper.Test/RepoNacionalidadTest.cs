using Spotify.Core;
using Spotify.Core.Persistencia;
namespace Spotify.ReposDapper.Test;

public class RepoNacionalidadTest : TestBase
{
    IRepoNacionalidad _repo;
    
    public RepoNacionalidadTest() : base()
        => _repo = new RepoNacionalidad(Conexion);
        
    [Fact]
    public void ListarOK()
    {
        var nacionalidades = _repo.Obtener();
        Assert.Contains(nacionalidades, n=> n.Pais == "Argentina");
    }

    [Fact]
    public void AltaNacionalidadOK()
    {
        var nuevaNacionalidad = new Nacionalidad {Pais = "España"};
        var IdNAcionalidad = _repo.Alta(nuevaNacionalidad);

        var nacionalidades = _repo.Obtener();
        Assert.Contains(nacionalidades, n => n.Pais == "España");
    }

}
