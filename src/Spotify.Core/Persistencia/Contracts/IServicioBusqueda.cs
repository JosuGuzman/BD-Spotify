using Spotify.Core.Models.Busqueda;

namespace Spotify.Core.Persistencia.Contracts;

public interface IServicioBusqueda
{
    Task<ResultadoBusqueda> BuscarGlobalAsync(string termino, int limite = 20, CancellationToken cancellationToken = default);
    Task<ResultadoBusqueda> BuscarAvanzadoAsync(FiltroBusqueda filtro, CancellationToken cancellationToken = default);
}