using Spotify.Core;
using Spotify.Core.Persistencia;
namespace Spotify.ReposDapper.Test;

public class RepoGeneroTest : TestBase
{
    IRepoGenero _repo;
    
    public RepoGeneroTest() : base()
        => _repo = new RepoGenero(Conexion);
    
    [Fact]
    public void ListarOK()
    {
        var generos = _repo.Obtener();
        Assert.Contains(generos, g=> g.genero == "Jazz");
    }

   [Fact]
    public void AltaGeneroOK()
    {
        var nuevoGenero = new Genero { genero = "Rock" };
        var IdGenero = _repo.Alta(nuevoGenero);
        
        var generos = _repo.Obtener();
        Assert.Contains(generos, g => g.genero == "Rock");
    }

    [Fact]
    public void EleminarGeneroOK()
    {
        var generos = _repo.Obtener();
        var generoAEliminar = generos.FirstOrDefault(g => g.genero == "Jazz");

        if (generoAEliminar != null)
        {
            _repo.Eliminar(generoAEliminar.IdGenero);
            generos = _repo.Obtener();
            Assert.DoesNotContain(generos, g => g.genero == "Jazz");
        }
    }

}
