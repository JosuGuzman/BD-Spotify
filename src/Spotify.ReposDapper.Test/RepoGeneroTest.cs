using Spotify.Core.Persistencia;

namespace Spotify.ReposDapper.Test;

public class RepoGeneroTest : TestBase
{
    IRepoGenero _repo;
    public RepoGeneroTest() : base() => _repo = new RepoGenero(Conexion);

    [Fact]
    public void ListarOK()
    {
        var generos = _repo.Obtener();
        Assert.Contains(generos, g=> g.genero == "Jazz");
    }
}
