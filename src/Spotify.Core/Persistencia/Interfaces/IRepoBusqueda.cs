using System.Linq.Expressions;

namespace Spotify.Core.Persistencia;
    /// Interfaz para repositorios con operaciones específicas de búsqueda
public interface IRepoBusqueda<T> where T : class
{
    IEnumerable<T> BuscarTexto(string termino, params Expression<Func<T, object>>[] propiedades);
    Task<IEnumerable<T>> BuscarTextoAsync(string termino, params Expression<Func<T, object>>[] propiedades);
        
    IEnumerable<T> ObtenerPaginado(int pagina, int tamañoPagina, string ordenarPor = "Id");
    Task<IEnumerable<T>> ObtenerPaginadoAsync(int pagina, int tamañoPagina, string ordenarPor = "Id", CancellationToken cancellationToken = default);
        
    IEnumerable<T> ObtenerConRelaciones(params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> ObtenerConRelacionesAsync(params Expression<Func<T, object>>[] includes);
}