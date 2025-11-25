using System.Linq.Expressions;

namespace Spotify.Core.Persistencia;
/// Interfaz base para operaciones CRUD genéricas con mejores prácticas

public interface IRepoBase<T> where T : class
{
    // Operaciones síncronas
    T? ObtenerPorId(object id);
    IEnumerable<T> ObtenerTodos();
    IEnumerable<T> Buscar(Expression<Func<T, bool>> predicado);
    void Insertar(T entidad);
    void Actualizar(T entidad);
    void Eliminar(object id);
    void Eliminar(T entidad);
    
    // Operaciones asíncronas
    Task<T?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado, CancellationToken cancellationToken = default);
    Task InsertarAsync(T entidad, CancellationToken cancellationToken = default);
    Task ActualizarAsync(T entidad, CancellationToken cancellationToken = default);
    Task EliminarAsync(object id, CancellationToken cancellationToken = default);
    
    // Operaciones avanzadas
    int Contar();
    Task<int> ContarAsync(CancellationToken cancellationToken = default);
    bool Existe(object id);
    Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default);
}