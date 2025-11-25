namespace Spotify.ReposDapper;

public abstract class RepoGenerico
{
    private readonly string _connectionString;
    private readonly ILogger _logger;

    protected RepoGenerico(string connectionString, ILogger logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    protected IDbConnection CreateConnection()
    {
        var connection = new MySqlConnection(_connectionString);
        
        // Log de apertura de conexión (solo en desarrollo)
        _logger.LogDebug("Creando conexión a BD: {DataSource}", connection.DataSource);
        
        return connection;
    }

    protected DynamicParameters CreateParameters()
        => new DynamicParameters();

    protected void LogQuery(string operation, string query, object parameters = null)
    {
        _logger.LogDebug("Ejecutando {Operation}: {Query} con parámetros: {@Parameters}", 
            operation, query, parameters);
    }

    protected void LogExecutionTime(string operation, TimeSpan elapsed)
    {
        _logger.LogDebug("{Operation} ejecutado en {ElapsedMs}ms", operation, elapsed.TotalMilliseconds);
    }
}