using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Spotify.Core.Persistencia;
using Spotify.Core.Services;

namespace Spotify.Core.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IRepoArtista _repoArtista;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(IRepoArtista repoArtista, ILogger<DatabaseHealthCheck> logger)
    {
        _repoArtista = repoArtista;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Verificando conexión a la base de datos...");
            
            var artistas = await _repoArtista.ObtenerTodosAsync(cancellationToken);
            
            _logger.LogInformation("Health check de base de datos exitoso. {Count} artistas encontrados", artistas.Count());
            
            return artistas.Any() 
                ? HealthCheckResult.Healthy("Base de datos conectada y con datos")
                : HealthCheckResult.Degraded("Base de datos conectada pero sin datos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en health check de base de datos");
            return HealthCheckResult.Unhealthy("Error conectando a la base de datos", ex);
        }
    }
}

public class FileSystemHealthCheck : IHealthCheck
{
    private readonly FileService _fileService;
    private readonly ILogger<FileSystemHealthCheck> _logger;

    public FileSystemHealthCheck(FileService fileService, ILogger<FileSystemHealthCheck> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Verificando sistema de archivos...");

            // Verificar que podemos acceder a los directorios principales
            var portadasPath = Path.Combine(_fileService.GetUploadPath(), "portadas");
            var cancionesPath = Path.Combine(_fileService.GetUploadPath(), "canciones");

            if (!Directory.Exists(portadasPath) || !Directory.Exists(cancionesPath))
            {
                _logger.LogWarning("Directorios de uploads no existen");
                return Task.FromResult(HealthCheckResult.Degraded("Directorios de uploads no existen"));
            }

            // Verificar permisos de escritura
            var testFile = Path.Combine(portadasPath, $"healthcheck_{Guid.NewGuid()}.tmp");
            try
            {
                File.WriteAllText(testFile, "healthcheck");
                File.Delete(testFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sin permisos de escritura en directorio de uploads");
                return Task.FromResult(HealthCheckResult.Unhealthy("Sin permisos de escritura en directorio de uploads", ex));
            }

            _logger.LogInformation("Health check de sistema de archivos exitoso");
            return Task.FromResult(HealthCheckResult.Healthy("Sistema de archivos funcionando correctamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en health check de sistema de archivos");
            return Task.FromResult(HealthCheckResult.Unhealthy("Error en sistema de archivos", ex));
        }
    }
}

public class CacheHealthCheck : IHealthCheck
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheHealthCheck> _logger;

    public CacheHealthCheck(ICacheService cacheService, ILogger<CacheHealthCheck> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Verificando servicio de caché...");

            var testKey = $"healthcheck_{Guid.NewGuid()}";
            var testValue = "test_value";
            
            // Probar escritura
            _cacheService.Set(testKey, testValue, TimeSpan.FromSeconds(30));
            
            // Probar lectura
            var retrievedValue = _cacheService.Get<string>(testKey);
            
            if (retrievedValue == testValue)
            {
                _logger.LogInformation("Health check de caché exitoso");
                return HealthCheckResult.Healthy("Servicio de caché funcionando correctamente");
            }
            else
            {
                _logger.LogWarning("Health check de caché: valores no coinciden");
                return HealthCheckResult.Degraded("Servicio de caché con problemas de consistencia");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en health check de caché");
            return HealthCheckResult.Unhealthy("Error en servicio de caché", ex);
        }
    }
}