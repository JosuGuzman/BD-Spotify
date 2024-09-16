using Spotify.Core;
using Spotify.Core.Persistencia;
namespace Spotify.ReposDapper.Test;

public class RepoNacionalidadTest : TestBase
{
    IRepoNacionalidad _repo;
    
    public RepoNacionalidadTest() : base()
        => _repo = new RepoNacionalidad(Conexion);
        
    [Fact]
    public void Test1()
    {
        Assert.True(true);
    }
}
