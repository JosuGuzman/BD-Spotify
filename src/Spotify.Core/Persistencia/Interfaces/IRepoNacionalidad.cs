namespace Spotify.Core.Persistencia;
public interface IRepoNacionalidad : IRepoBase<Nacionalidad>
{
        // Operaciones específicas de Nacionalidad
    Nacionalidad? ObtenerPorPais(string pais);
    Task<Nacionalidad?> ObtenerPorPaisAsync(string pais, CancellationToken cancellationToken = default);
        
    IEnumerable<Nacionalidad> ObtenerPaisesConUsuarios();
    Task<IEnumerable<Nacionalidad>> ObtenerPaisesConUsuariosAsync(CancellationToken cancellationToken = default);
}