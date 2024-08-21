using System.Numerics;

namespace Spotify.Core.Persistencia;
/// <summary>
/// Obtener un elemento en base a su Id
/// </summary>
/// <typeparam name="T">Tipo del elemento a traer</typeparam>
/// <typeparam name="N">Tipo del indice</typeparam>
/// 
///Una interfaz que detalla un tipo de 
///busqueda que acepta argumentos de cualquier tipo osea que puedes buscar cualquier cosa dentro de la CRUD
public interface IDetallePorId<T, N> where N : IBinaryNumber<N>
{
    T? DetalleDe (N id);
}