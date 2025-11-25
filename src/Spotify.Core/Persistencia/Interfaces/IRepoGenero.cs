namespace Spotify.Core.Persistencia;

public interface IRepoGenero : IRepoBase<Genero>, IRepoBusqueda<Genero>
{
        // Operaciones específicas de Genero
    IEnumerable<Genero> ObtenerGenerosPopulares();
    Task<IEnumerable<Genero>> ObtenerGenerosPopularesAsync(CancellationToken cancellationToken = default);
        
    Genero? ObtenerPorNombre(string nombre);
    Task<Genero?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default);
}