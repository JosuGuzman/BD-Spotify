namespace Spotify.Core.Persistencia;
public interface IRepoUsuario : IRepoBase<Usuario>
{
        // Operaciones específicas de Usuario
    Usuario? ObtenerPorEmail(string email);
    Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken = default);
        
    Usuario? ObtenerPorNombreUsuario(string nombreUsuario);
    Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken cancellationToken = default);
        
    Usuario? ObtenerConPlaylists(uint idUsuario);
    Task<Usuario?> ObtenerConPlaylistsAsync(uint idUsuario, CancellationToken cancellationToken = default);
        
    Usuario? ObtenerConSuscripciones(uint idUsuario);
    Task<Usuario?> ObtenerConSuscripcionesAsync(uint idUsuario, CancellationToken cancellationToken = default);
        
    bool VerificarCredenciales(string email, string contraseña);
    Task<bool> VerificarCredencialesAsync(string email, string contraseña, CancellationToken cancellationToken = default);
        
    Task<bool> CambiarContraseñaAsync(uint idUsuario, string nuevaContraseña, CancellationToken cancellationToken = default);
}