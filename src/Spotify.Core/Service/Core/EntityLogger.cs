using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Spotify.Core.Services;

public interface IEntityLogger<T>
{
    void LogEntityCreated(T entity);
    void LogEntityUpdated(T entity);
    void LogEntityDeleted(T entity);
    void LogEntityError(T entity, string operation, Exception exception);
    void LogEntityAccess(T entity, string operation);
}

public class EntityLogger<T> : IEntityLogger<T>
{
    private readonly ILogger<EntityLogger<T>> _logger;

    public EntityLogger(ILogger<EntityLogger<T>> logger)
    {
        _logger = logger;
    }

    public void LogEntityCreated(T entity)
    {
        _logger.LogInformation("Entity {EntityType} created: {EntityId}", 
            typeof(T).Name, GetEntityId(entity));
    }

    public void LogEntityUpdated(T entity)
    {
        _logger.LogInformation("Entity {EntityType} updated: {EntityId}", 
            typeof(T).Name, GetEntityId(entity));
    }

    public void LogEntityDeleted(T entity)
    {
        _logger.LogInformation("Entity {EntityType} deleted: {EntityId}", 
            typeof(T).Name, GetEntityId(entity));
    }

    public void LogEntityError(T entity, string operation, Exception exception)
    {
        _logger.LogError(exception, "Error {Operation} entity {EntityType}: {EntityId}", 
            operation, typeof(T).Name, GetEntityId(entity));
    }

    public void LogEntityAccess(T entity, string operation)
    {
        _logger.LogDebug("Entity {EntityType} accessed: {Operation} - {EntityId}", 
            typeof(T).Name, operation, GetEntityId(entity));
    }

    private static string GetEntityId(T entity)
    {
        return entity?.GetType().GetProperty("id")?.GetValue(entity)?.ToString() 
            ?? entity?.GetHashCode().ToString() 
            ?? "unknown";
    }
}