global using System.Data;
global using Spotify.Core;
global using Dapper;
global using Spotify.Core.Persistencia;
global using Microsoft.Extensions.Logging;
global using System.Threading;
global using MySql.Data.MySqlClient;
global using Microsoft.Extensions.Logging;
global using System.Linq.Expressions;

namespace Spotify.ReposDapper;

public class RepoAlbum : RepoGenerico, IRepoAlbum
{
    public RepoAlbum(string connectionString, ILogger<RepoAlbum> logger) 
        : base(connectionString, logger) { }

    public Album? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT a.*, art.*
            FROM Album a
            JOIN Artista art ON a.idArtista = art.idArtista
            WHERE a.idAlbum = @id";

        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Album, Artista, Album>(sql,
            (album, artista) =>
            {
                album.Artista = artista;
                return album;
            }, new { id }, splitOn: "idArtista").FirstOrDefault();
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Album> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerAlbum");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Album>("ObtenerAlbum", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Album> Buscar(Expression<Func<Album, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Album entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unTitulo", entidad.Titulo);
        parameters.Add("unidArtista", entidad.Artista.idArtista);
        parameters.Add("unPortada", entidad.Portada);
        parameters.Add("unidAlbum", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaAlbum", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaAlbum", parameters, commandType: CommandType.StoredProcedure);
        entidad.idAlbum = parameters.Get<uint>("unidAlbum");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Album entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Album 
                   SET Titulo = @Titulo,
                       fechaLanzamiento = @FechaLanzamiento,
                       idArtista = @idArtista,
                       Portada = @Portada
                   WHERE idAlbum = @idAlbum";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            entidad.Titulo,
            entidad.FechaLanzamiento,
            idArtista = entidad.Artista.idArtista,
            entidad.Portada,
            entidad.idAlbum
        });
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        LogQuery("Eliminar", "eliminarAlbum", new { unidAlbum = id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("eliminarAlbum", new { unidAlbum = id }, 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Album entidad)
    {
        Eliminar(entidad.idAlbum);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Album";
        
        LogQuery("Contar", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("Contar", stopwatch.Elapsed);
        return result;
    }

    public bool Existe(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Album WHERE idAlbum = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

        // Implementación de IRepositorioBusqueda<Album>
    public IEnumerable<Album> BuscarTexto(string termino, params Expression<Func<Album, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Album WHERE Titulo LIKE @termino";
        
        LogQuery("BuscarTexto", sql, new { termino = $"%{termino}%" });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Album>(sql, new { termino = $"%{termino}%" });
        stopwatch.Stop();
        
        LogExecutionTime("BuscarTexto", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Album> ObtenerPaginado(int pagina, int tamañoPagina, string ordenarPor = "idAlbum")
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Album ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
        
        LogQuery("ObtenerPaginado", sql, new { tamañoPagina, offset });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Album>(sql, new { tamañoPagina, offset });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaginado", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Album> ObtenerConRelaciones(params Expression<Func<Album, object>>[] includes)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT a.*, art.*, c.*
            FROM Album a
            JOIN Artista art ON a.idArtista = art.idArtista
            LEFT JOIN Cancion c ON a.idAlbum = c.idAlbum";

        LogQuery("ObtenerConRelaciones", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var albumes = new Dictionary<uint, Album>();
        var result = connection.Query<Album, Artista, Cancion, Album>(sql,
            (album, artista, cancion) =>
            {
                if (!albumes.TryGetValue(album.idAlbum, out var albumEntry))
                {
                    albumEntry = album;
                    albumEntry.Artista = artista;
                    albumEntry.Canciones = new List<Cancion>();
                    albumes.Add(albumEntry.idAlbum, albumEntry);
                }
                
                if (cancion != null)
                {
                    albumEntry.Canciones.Add(cancion);
                }
                
                return albumEntry;
            }, splitOn: "idArtista,idCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelaciones", stopwatch.Elapsed);
        
        return albumes.Values;
    }

        // Métodos específicos de IRepositorioAlbum
    public IEnumerable<Album> ObtenerAlbumesRecientes(int limite = 10)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT a.*, art.*
                   FROM Album a
                   JOIN Artista art ON a.idArtista = art.idArtista
                   ORDER BY a.fechaLanzamiento DESC
                   LIMIT @limite";

        LogQuery("ObtenerAlbumesRecientes", sql, new { limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Album, Artista, Album>(sql,
            (album, artista) =>
            {
                album.Artista = artista;
                return album;
            }, new { limite }, splitOn: "idArtista");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerAlbumesRecientes", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Album> ObtenerPorArtista(uint idArtista)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT a.*, art.*
            FROM Album a
            JOIN Artista art ON a.idArtista = art.idArtista
            WHERE a.idArtista = @idArtista";

        LogQuery("ObtenerPorArtista", sql, new { idArtista });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Album, Artista, Album>(sql,
            (album, artista) =>
            {
                album.Artista = artista;
                return album;
            }, new { idArtista }, splitOn: "idArtista");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorArtista", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Album> ObtenerConCanciones()
    {
        return ObtenerConRelaciones();
    }

    public Album? ObtenerConArtistaYCanciones(uint idAlbum)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT a.*, art.*, c.*, g.*, artCancion.*
            FROM Album a
            JOIN Artista art ON a.idArtista = art.idArtista
            LEFT JOIN Cancion c ON a.idAlbum = c.idAlbum
            LEFT JOIN Genero g ON c.idGenero = g.idGenero
            LEFT JOIN Artista artCancion ON c.idArtista = artCancion.idArtista
            WHERE a.idAlbum = @idAlbum";

        LogQuery("ObtenerConArtistaYCanciones", sql, new { idAlbum });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var albumes = new Dictionary<uint, Album>();
        var result = connection.Query<Album, Artista, Cancion, Genero, Artista, Album>(sql,
            (album, artista, cancion, genero, artistaCancion) =>
            {
                if (!albumes.TryGetValue(album.idAlbum, out var albumEntry))
                {
                    albumEntry = album;
                    albumEntry.Artista = artista;
                    albumEntry.Canciones = new List<Cancion>();
                    albumes.Add(albumEntry.idAlbum, albumEntry);
                }
                
                if (cancion != null)
                {
                    cancion.Genero = genero;
                    cancion.Artista = artistaCancion;
                    albumEntry.Canciones.Add(cancion);
                }
                
                return albumEntry;
            }, new { idAlbum }, splitOn: "idArtista,idCancion,idGenero,idArtista");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConArtistaYCanciones", stopwatch.Elapsed);
        
        return albumes.Values.FirstOrDefault();
    }

    public async Task<Album?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT a.*, art.*
            FROM Album a
            JOIN Artista art ON a.idArtista = art.idArtista
            WHERE a.idAlbum = @id";
            
        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Album, Artista, Album>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (album, artista) =>
            {
                album.Artista = artista;
                return album;
            }, splitOn: "idArtista");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Album>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerAlbum");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Album>(
            new CommandDefinition("ObtenerAlbum", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Album>> BuscarAsync(Expression<Func<Album, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Album entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unTitulo", entidad.Titulo);
        parameters.Add("unidArtista", entidad.Artista.idArtista);
        parameters.Add("unPortada", entidad.Portada);
        parameters.Add("unidAlbum", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaAlbum", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaAlbum", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.idAlbum = parameters.Get<uint>("unidAlbum");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Album entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Album 
                   SET Titulo = @Titulo,
                       fechaLanzamiento = @FechaLanzamiento,
                       idArtista = @idArtista,
                       Portada = @Portada
                   WHERE idAlbum = @idAlbum";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                entidad.Titulo,
                entidad.FechaLanzamiento,
                idArtista = entidad.Artista.idArtista,
                entidad.Portada,
                entidad.idAlbum
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("EliminarAsync", "eliminarAlbum", new { unidAlbum = id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("eliminarAlbum", new { unidAlbum = id }, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Album";
        
        LogQuery("ContarAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ContarAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Album WHERE idAlbum = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Album>> BuscarTextoAsync(string termino, params Expression<Func<Album, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Album WHERE Titulo LIKE @termino";
        
        LogQuery("BuscarTextoAsync", sql, new { termino = $"%{termino}%" });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Album>(
            new CommandDefinition(sql, new { termino = $"%{termino}%" }, 
                cancellationToken: CancellationToken.None));
        stopwatch.Stop();
        
        LogExecutionTime("BuscarTextoAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Album>> ObtenerPaginadoAsync(int pagina, int tamañoPagina, string ordenarPor = "idAlbum", CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Album ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
        
        LogQuery("ObtenerPaginadoAsync", sql, new { tamañoPagina, offset });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Album>(
            new CommandDefinition(sql, new { tamañoPagina, offset }, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaginadoAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Album>> ObtenerConRelacionesAsync(params Expression<Func<Album, object>>[] includes)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT a.*, art.*, c.*
            FROM Album a
            JOIN Artista art ON a.idArtista = art.idArtista
            LEFT JOIN Cancion c ON a.idAlbum = c.idAlbum";

        LogQuery("ObtenerConRelacionesAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var albumes = new Dictionary<uint, Album>();
        var result = await connection.QueryAsync<Album, Artista, Cancion, Album>(
            new CommandDefinition(sql, cancellationToken: CancellationToken.None),
            (album, artista, cancion) =>
            {
                if (!albumes.TryGetValue(album.idAlbum, out var albumEntry))
                {
                    albumEntry = album;
                    albumEntry.Artista = artista;
                    albumEntry.Canciones = new List<Cancion>();
                    albumes.Add(albumEntry.idAlbum, albumEntry);
                }
                
                if (cancion != null)
                {
                    albumEntry.Canciones.Add(cancion);
                }
                
                return albumEntry;
            }, splitOn: "idArtista,idCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelacionesAsync", stopwatch.Elapsed);
        
        return albumes.Values;
    }

    public async Task<IEnumerable<Album>> ObtenerAlbumesRecientesAsync(int limite = 10, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT a.*, art.*
                   FROM Album a
                   JOIN Artista art ON a.idArtista = art.idArtista
                   ORDER BY a.fechaLanzamiento DESC
                   LIMIT @limite";
                   
        LogQuery("ObtenerAlbumesRecientesAsync", sql, new { limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Album, Artista, Album>(
            new CommandDefinition(sql, new { limite }, cancellationToken: cancellationToken),
            (album, artista) =>
            {
                album.Artista = artista;
                return album;
            }, splitOn: "idArtista");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerAlbumesRecientesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Album>> ObtenerPorArtistaAsync(uint idArtista, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT a.*, art.*
            FROM Album a
            JOIN Artista art ON a.idArtista = art.idArtista
            WHERE a.idArtista = @idArtista";

        LogQuery("ObtenerPorArtistaAsync", sql, new { idArtista });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Album, Artista, Album>(
            new CommandDefinition(sql, new { idArtista }, cancellationToken: cancellationToken),
            (album, artista) =>
            {
                album.Artista = artista;
                return album;
            }, splitOn: "idArtista");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorArtistaAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Album>> ObtenerConCancionesAsync(CancellationToken cancellationToken = default)
    {
        return await ObtenerConRelacionesAsync();
    }

    public async Task<Album?> ObtenerConArtistaYCancionesAsync(uint idAlbum, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT a.*, art.*, c.*, g.*, artCancion.*
            FROM Album a
            JOIN Artista art ON a.idArtista = art.idArtista
            LEFT JOIN Cancion c ON a.idAlbum = c.idAlbum
            LEFT JOIN Genero g ON c.idGenero = g.idGenero
            LEFT JOIN Artista artCancion ON c.idArtista = artCancion.idArtista
            WHERE a.idAlbum = @idAlbum";

        LogQuery("ObtenerConArtistaYCancionesAsync", sql, new { idAlbum });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var albumes = new Dictionary<uint, Album>();
        var result = await connection.QueryAsync<Album, Artista, Cancion, Genero, Artista, Album>(
            new CommandDefinition(sql, new { idAlbum }, cancellationToken: cancellationToken),
            (album, artista, cancion, genero, artistaCancion) =>
            {
                if (!albumes.TryGetValue(album.idAlbum, out var albumEntry))
                {
                    albumEntry = album;
                    albumEntry.Artista = artista;
                    albumEntry.Canciones = new List<Cancion>();
                    albumes.Add(albumEntry.idAlbum, albumEntry);
                }
                
                if (cancion != null)
                {
                    cancion.Genero = genero;
                    cancion.Artista = artistaCancion;
                    albumEntry.Canciones.Add(cancion);
                }
                
                return albumEntry;
            }, splitOn: "idArtista,idCancion,idGenero,idArtista");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConArtistaYCancionesAsync", stopwatch.Elapsed);
        
        return albumes.Values.FirstOrDefault();
    }
}

public class RepoArtista : RepoGenerico, IRepoArtista
{
    public RepoArtista(string connectionString, ILogger<RepoArtista> logger) 
        : base(connectionString, logger) { }

    public Artista? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Artista WHERE idArtista = @id";
        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<Artista>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Artista> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerArtistas");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Artista>("ObtenerArtistas", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Artista> Buscar(Expression<Func<Artista, bool>> predicado)
    {
        // Para Dapper, necesitamos convertir la expresión a SQL manualmente
        // Esta es una implementación simplificada
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Artista entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unNombreArtistico", entidad.NombreArtistico);
        parameters.Add("unNombre", entidad.Nombre);
        parameters.Add("unApellido", entidad.Apellido);
        parameters.Add("unidArtista", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaArtista", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaArtista", parameters, commandType: CommandType.StoredProcedure);
        entidad.idArtista = parameters.Get<uint>("unidArtista");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Artista entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Artista 
                       SET NombreArtistico = @NombreArtistico, 
                           Nombre = @Nombre, 
                           Apellido = @Apellido 
                       WHERE idArtista = @idArtista";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, entidad);
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        LogQuery("Eliminar", "eliminarArtista", new { unidArtista = id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("eliminarArtista", new { unidArtista = id }, 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Artista entidad)
    {
        Eliminar(entidad.idArtista);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Artista";
        
        LogQuery("Contar", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("Contar", stopwatch.Elapsed);
        return result;
    }

    public bool Existe(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Artista WHERE idArtista = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    // Implementación específica de IRepositorioBusqueda<Artista>
    public IEnumerable<Artista> BuscarTexto(string termino, params Expression<Func<Artista, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT * FROM Artista 
                       WHERE NombreArtistico LIKE @termino 
                          OR Nombre LIKE @termino 
                          OR Apellido LIKE @termino";
        
        LogQuery("BuscarTexto", sql, new { termino = $"%{termino}%" });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Artista>(sql, new { termino = $"%{termino}%" });
        stopwatch.Stop();
        
        LogExecutionTime("BuscarTexto", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Artista> ObtenerPaginado(int pagina, int tamañoPagina, string ordenarPor = "idArtista")
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Artista ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
        
        LogQuery("ObtenerPaginado", sql, new { tamañoPagina, offset });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Artista>(sql, new { tamañoPagina, offset });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaginado", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Artista> ObtenerConRelaciones(params Expression<Func<Artista, object>>[] includes)
    {
        // Implementación básica - carga albumes por defecto
        using var connection = CreateConnection();
        var sql = @"SELECT a.*, al.* 
                       FROM Artista a 
                       LEFT JOIN Album al ON a.idArtista = al.idArtista";
        
        LogQuery("ObtenerConRelaciones", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var artistas = new Dictionary<uint, Artista>();
        
        var result = connection.Query<Artista, Album, Artista>(sql, 
            (artista, album) => 
            {
                if (!artistas.TryGetValue(artista.idArtista, out var artistaEntry))
                {
                    artistaEntry = artista;
                    artistaEntry.Albumes = new List<Album>();
                    artistas.Add(artistaEntry.idArtista, artistaEntry);
                }
                
                if (album != null)
                {
                    artistaEntry.Albumes.Add(album);
                }
                
                return artistaEntry;
            }, splitOn: "idAlbum");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelaciones", stopwatch.Elapsed);
        
        return artistas.Values;
    }

    // Métodos específicos de IRepositorioArtista
    public IEnumerable<Artista> ObtenerArtistasPopulares(int limite = 10)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT a.*, COUNT(r.idHistorial) as TotalReproducciones
                       FROM Artista a
                       JOIN Cancion c ON a.idArtista = c.idArtista
                       JOIN HistorialReproduccion r ON c.idCancion = r.idCancion
                       GROUP BY a.idArtista
                       ORDER BY TotalReproducciones DESC
                       LIMIT @limite";
        
        LogQuery("ObtenerArtistasPopulares", sql, new { limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Artista>(sql, new { limite });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerArtistasPopulares", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Artista> ObtenerConAlbumes()
    {
        return ObtenerConRelaciones();
    }

    public Artista? ObtenerPorNombreArtistico(string nombreArtistico)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Artista WHERE NombreArtistico = @nombreArtistico";
        
        LogQuery("ObtenerPorNombreArtistico", sql, new { nombreArtistico });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<Artista>(sql, new { nombreArtistico });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorNombreArtistico", stopwatch.Elapsed);
        return result;
    }
    
    public async Task<Artista?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Artista WHERE idArtista = @id";
        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<Artista>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Artista>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerArtistas");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Artista>(
            new CommandDefinition("ObtenerArtistas", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Artista>> BuscarAsync(Expression<Func<Artista, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Artista entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unNombreArtistico", entidad.NombreArtistico);
        parameters.Add("unNombre", entidad.Nombre);
        parameters.Add("unApellido", entidad.Apellido);
        parameters.Add("unidArtista", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaArtista", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaArtista", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.idArtista = parameters.Get<uint>("unidArtista");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Artista entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Artista 
                       SET NombreArtistico = @NombreArtistico, 
                           Nombre = @Nombre, 
                           Apellido = @Apellido 
                       WHERE idArtista = @idArtista";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, entidad, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("EliminarAsync", "eliminarArtista", new { unidArtista = id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("eliminarArtista", new { unidArtista = id }, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Artista";
        
        LogQuery("ContarAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ContarAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Artista WHERE idArtista = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Artista>> BuscarTextoAsync(string termino, params Expression<Func<Artista, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT * FROM Artista 
                       WHERE NombreArtistico LIKE @termino 
                          OR Nombre LIKE @termino 
                          OR Apellido LIKE @termino";
        
        LogQuery("BuscarTextoAsync", sql, new { termino = $"%{termino}%" });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Artista>(
            new CommandDefinition(sql, new { termino = $"%{termino}%" }, 
                cancellationToken: CancellationToken.None));
        stopwatch.Stop();
        
        LogExecutionTime("BuscarTextoAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Artista>> ObtenerPaginadoAsync(int pagina, int tamañoPagina, string ordenarPor = "idArtista", CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Artista ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
        
        LogQuery("ObtenerPaginadoAsync", sql, new { tamañoPagina, offset });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Artista>(
            new CommandDefinition(sql, new { tamañoPagina, offset }, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaginadoAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Artista>> ObtenerConRelacionesAsync(params Expression<Func<Artista, object>>[] includes)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT a.*, al.* 
                       FROM Artista a 
                       LEFT JOIN Album al ON a.idArtista = al.idArtista";
        
        LogQuery("ObtenerConRelacionesAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var artistas = new Dictionary<uint, Artista>();
        var result = await connection.QueryAsync<Artista, Album, Artista>(
            new CommandDefinition(sql, cancellationToken: CancellationToken.None),
            (artista, album) => 
            {
                if (!artistas.TryGetValue(artista.idArtista, out var artistaEntry))
                {
                    artistaEntry = artista;
                    artistaEntry.Albumes = new List<Album>();
                    artistas.Add(artistaEntry.idArtista, artistaEntry);
                }
                
                if (album != null)
                {
                    artistaEntry.Albumes.Add(album);
                }
                
                return artistaEntry;
            }, splitOn: "idAlbum");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelacionesAsync", stopwatch.Elapsed);
        
        return artistas.Values;
    }

    public async Task<IEnumerable<Artista>> ObtenerArtistasPopularesAsync(int limite = 10, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT a.*, COUNT(r.idHistorial) as TotalReproducciones
                       FROM Artista a
                       JOIN Cancion c ON a.idArtista = c.idArtista
                       JOIN HistorialReproduccion r ON c.idCancion = r.idCancion
                       GROUP BY a.idArtista
                       ORDER BY TotalReproducciones DESC
                       LIMIT @limite";
        
        LogQuery("ObtenerArtistasPopularesAsync", sql, new { limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Artista>(
            new CommandDefinition(sql, new { limite }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerArtistasPopularesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Artista>> ObtenerConAlbumesAsync(CancellationToken cancellationToken = default)
    {
        return await ObtenerConRelacionesAsync();
    }

    public async Task<Artista?> ObtenerPorNombreArtisticoAsync(string nombreArtistico, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Artista WHERE NombreArtistico = @nombreArtistico";
        
        LogQuery("ObtenerPorNombreArtisticoAsync", sql, new { nombreArtistico });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<Artista>(
            new CommandDefinition(sql, new { nombreArtistico }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorNombreArtisticoAsync", stopwatch.Elapsed);
        return result;
    }
}

public class RepoCancion : RepoGenerico, IRepoCancion
{
    public RepoCancion(string connectionString, ILogger<RepoCancion> logger) 
        : base(connectionString, logger) { }

    // Implementación de IRepositorioBase<Cancion>
    public Cancion? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idCancion = @id";

        LogQuery("ObtenerPorId", sql, new { id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                cancion.Album = album;
                cancion.Genero = genero;
                cancion.Artista = artista;
                return cancion;
            }, new { id }, splitOn: "idAlbum,idGenero,idArtista").FirstOrDefault();
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Cancion> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerCanciones");
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>("ObtenerCanciones", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Cancion> Buscar(Expression<Func<Cancion, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Cancion entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unTitulo", entidad.Titulo);
        parameters.Add("unDuration", entidad.Duracion);
        parameters.Add("unidAlbum", entidad.Album.idAlbum);
        parameters.Add("unidArtista", entidad.Artista.idArtista);
        parameters.Add("unidGenero", entidad.Genero.idGenero);
        parameters.Add("unArchivoMP3", entidad.ArchivoMP3);
        parameters.Add("unidCancion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaCancion", parameters);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaCancion", parameters, commandType: CommandType.StoredProcedure);
        entidad.idCancion = parameters.Get<uint>("unidCancion");
        stopwatch.Stop();
    
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Cancion entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Cancion 
                       SET Titulo = @Titulo, 
                           Duracion = @Duracion,
                           idAlbum = @idAlbum,
                           idArtista = @idArtista,
                           idGenero = @idGenero,
                           ArchivoMP3 = @ArchivoMP3
                       WHERE idCancion = @idCancion";
    
        LogQuery("Actualizar", sql, entidad);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            entidad.Titulo,
            entidad.Duracion,
            idAlbum = entidad.Album.idAlbum,
            idArtista = entidad.Artista.idArtista,
            idGenero = entidad.Genero.idGenero,
            entidad.ArchivoMP3,
            entidad.idCancion
        });
        stopwatch.Stop();
    
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        LogQuery("Eliminar", "eliminarCancion", new { unidCancion = id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("eliminarCancion", new { unidCancion = id }, 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
    
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Cancion entidad)
    {
        Eliminar(entidad.idCancion);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Cancion";
    
        LogQuery("Contar", sql);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql);
        stopwatch.Stop();
    
        LogExecutionTime("Contar", stopwatch.Elapsed);
        return result;
    }

    public bool Existe(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Cancion WHERE idCancion = @id";
    
        LogQuery("Existe", sql, new { id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
    
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    // Implementación de IRepositorioBusqueda<Cancion>
    public IEnumerable<Cancion> BuscarTexto(string termino, params Expression<Func<Cancion, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Cancion WHERE Titulo LIKE @termino";
    
        LogQuery("BuscarTexto", sql, new { termino = $"%{termino}%" });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>(sql, new { termino = $"%{termino}%" });
        stopwatch.Stop();
    
        LogExecutionTime("BuscarTexto", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Cancion> ObtenerPaginado(int pagina, int tamañoPagina, string ordenarPor = "idCancion")
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Cancion ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
    
        LogQuery("ObtenerPaginado", sql, new { tamañoPagina, offset });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>(sql, new { tamañoPagina, offset });
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerPaginado", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Cancion> ObtenerConRelaciones(params Expression<Func<Cancion, object>>[] includes)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero";

        LogQuery("ObtenerConRelaciones", sql);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelaciones", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    // Métodos específicos de IRepositorioCancion
    public IEnumerable<Cancion> ObtenerCancionesPopulares(int limite = 10)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT c.*, COUNT(r.idHistorial) as TotalReproducciones
                       FROM Cancion c
                       LEFT JOIN HistorialReproduccion r ON c.idCancion = r.idCancion
                       GROUP BY c.idCancion
                       ORDER BY TotalReproducciones DESC
                       LIMIT @limite";
    
        LogQuery("ObtenerCancionesPopulares", sql, new { limite });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>(sql, new { limite });
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerCancionesPopulares", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Cancion> ObtenerPorAlbum(uint idAlbum)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idAlbum = @idAlbum";

        LogQuery("ObtenerPorAlbum", sql, new { idAlbum });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, new { idAlbum }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorAlbum", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public IEnumerable<Cancion> ObtenerPorArtista(uint idArtista)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idArtista = @idArtista";

        LogQuery("ObtenerPorArtista", sql, new { idArtista });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, new { idArtista }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorArtista", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public IEnumerable<Cancion> ObtenerPorGenero(byte idGenero)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idGenero = @idGenero";

        LogQuery("ObtenerPorGenero", sql, new { idGenero });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, new { idGenero }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorGenero", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public IEnumerable<Cancion> BuscarPorLetra(string texto)
    {
        // Implementación simplificada - en un caso real usarías full-text search
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Cancion WHERE Titulo LIKE @texto";
    
        LogQuery("BuscarPorLetra", sql, new { texto = $"%{texto}%" });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>(sql, new { texto = $"%{texto}%" });
        stopwatch.Stop();
    
        LogExecutionTime("BuscarPorLetra", stopwatch.Elapsed);
        return result;
    }

    public Cancion? ObtenerConAlbumYArtista(uint idCancion)
    {
        return ObtenerPorId(idCancion);
    }

    public int IncrementarReproducciones(uint idCancion)
    {
        // Esta operación normalmente se haría en el servicio de reproducción
        // Pero para el repositorio, podríamos tener un campo de contador
        using var connection = CreateConnection();
        var sql = "UPDATE Cancion SET Reproducciones = COALESCE(Reproducciones, 0) + 1 WHERE idCancion = @idCancion";
    
        LogQuery("IncrementarReproducciones", sql, new { idCancion });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new { idCancion });
        stopwatch.Stop();
    
        LogExecutionTime("IncrementarReproducciones", stopwatch.Elapsed);
        return result;
    }

    public async Task<Cancion?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idCancion = @id";

        LogQuery("ObtenerPorIdAsync", sql, new { id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (cancion, album, genero, artista) =>
            {
                cancion.Album = album;
                cancion.Genero = genero;
                cancion.Artista = artista;
                return cancion;
            }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
    
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Cancion>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerCanciones");
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion>(
            new CommandDefinition("ObtenerCanciones", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Cancion>> BuscarAsync(Expression<Func<Cancion, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Cancion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unTitulo", entidad.Titulo);
        parameters.Add("unDuration", entidad.Duracion);
        parameters.Add("unidAlbum", entidad.Album.idAlbum);
        parameters.Add("unidArtista", entidad.Artista.idArtista);
        parameters.Add("unidGenero", entidad.Genero.idGenero);
        parameters.Add("unArchivoMP3", entidad.ArchivoMP3);
        parameters.Add("unidCancion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaCancion", parameters);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaCancion", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.idCancion = parameters.Get<uint>("unidCancion");
        stopwatch.Stop();
    
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Cancion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Cancion 
                       SET Titulo = @Titulo, 
                           Duracion = @Duracion,
                           idAlbum = @idAlbum,
                           idArtista = @idArtista,
                           idGenero = @idGenero,
                           ArchivoMP3 = @ArchivoMP3
                       WHERE idCancion = @idCancion";
    
        LogQuery("ActualizarAsync", sql, entidad);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                entidad.Titulo,
                entidad.Duracion,
                idAlbum = entidad.Album.idAlbum,
                idArtista = entidad.Artista.idArtista,
                idGenero = entidad.Genero.idGenero,
                entidad.ArchivoMP3,
                entidad.idCancion
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("EliminarAsync", "eliminarCancion", new { unidCancion = id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("eliminarCancion", new { unidCancion = id }, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Cancion";
    
        LogQuery("ContarAsync", sql);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("ContarAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Cancion WHERE idCancion = @id";
    
        LogQuery("ExisteAsync", sql, new { id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Cancion>> BuscarTextoAsync(string termino, params Expression<Func<Cancion, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Cancion WHERE Titulo LIKE @termino";
    
        LogQuery("BuscarTextoAsync", sql, new { termino = $"%{termino}%" });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion>(
            new CommandDefinition(sql, new { termino = $"%{termino}%" }, 
                cancellationToken: CancellationToken.None));
        stopwatch.Stop();
    
        LogExecutionTime("BuscarTextoAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Cancion>> ObtenerPaginadoAsync(int pagina, int tamañoPagina, string ordenarPor = "idCancion", CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Cancion ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
    
        LogQuery("ObtenerPaginadoAsync", sql, new { tamañoPagina, offset });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion>(
            new CommandDefinition(sql, new { tamañoPagina, offset }, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerPaginadoAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Cancion>> ObtenerConRelacionesAsync(params Expression<Func<Cancion, object>>[] includes)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero";

        LogQuery("ObtenerConRelacionesAsync", sql);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, cancellationToken: CancellationToken.None),
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelacionesAsync", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public async Task<IEnumerable<Cancion>> ObtenerCancionesPopularesAsync(int limite = 10, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT c.*, COUNT(r.idHistorial) as TotalReproducciones
                       FROM Cancion c
                       LEFT JOIN HistorialReproduccion r ON c.idCancion = r.idCancion
                       GROUP BY c.idCancion
                       ORDER BY TotalReproducciones DESC
                       LIMIT @limite";
    
        LogQuery("ObtenerCancionesPopularesAsync", sql, new { limite });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion>(
            new CommandDefinition(sql, new { limite }, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerCancionesPopularesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Cancion>> ObtenerPorAlbumAsync(uint idAlbum, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idAlbum = @idAlbum";

        LogQuery("ObtenerPorAlbumAsync", sql, new { idAlbum });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, new { idAlbum }, cancellationToken: cancellationToken),
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorAlbumAsync", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public async Task<IEnumerable<Cancion>> ObtenerPorArtistaAsync(uint idArtista, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idArtista = @idArtista";

        LogQuery("ObtenerPorArtistaAsync", sql, new { idArtista });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, new { idArtista }, cancellationToken: cancellationToken),
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorArtistaAsync", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public async Task<IEnumerable<Cancion>> ObtenerPorGeneroAsync(byte idGenero, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idGenero = @idGenero";

        LogQuery("ObtenerPorGeneroAsync", sql, new { idGenero });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, new { idGenero }, cancellationToken: cancellationToken),
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorGeneroAsync", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public async Task<IEnumerable<Cancion>> BuscarPorLetraAsync(string texto, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Cancion WHERE Titulo LIKE @texto";
    
        LogQuery("BuscarPorLetraAsync", sql, new { texto = $"%{texto}%" });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion>(
            new CommandDefinition(sql, new { texto = $"%{texto}%" }, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("BuscarPorLetraAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<Cancion?> ObtenerConAlbumYArtistaAsync(uint idCancion, CancellationToken cancellationToken = default)
    {
        return await ObtenerPorIdAsync(idCancion, cancellationToken);
    }

    public async Task<int> IncrementarReproduccionesAsync(uint idCancion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Cancion SET Reproducciones = COALESCE(Reproducciones, 0) + 1 WHERE idCancion = @idCancion";
    
        LogQuery("IncrementarReproduccionesAsync", sql, new { idCancion });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { idCancion }, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("IncrementarReproduccionesAsync", stopwatch.Elapsed);
        return result;
    }
}

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

public class RepoGenero : RepoGenerico, IRepoGenero
{
    public RepoGenero(string connectionString, ILogger<RepoGenero> logger) 
        : base(connectionString, logger) { }
    public Genero? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Genero WHERE idGenero = @id";
        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<Genero>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }
    public IEnumerable<Genero> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerGeneros");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Genero>("ObtenerGeneros", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }
    public IEnumerable<Genero> Buscar(Expression<Func<Genero, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }
    public void Insertar(Genero entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unGenero", entidad.Nombre);
        parameters.Add("unDescripcion", entidad.Descripcion);
        parameters.Add("unidGenero", dbType: DbType.Byte, direction: ParameterDirection.Output);
        LogQuery("Insertar", "altaGenero", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaGenero", parameters, commandType: CommandType.StoredProcedure);
        entidad.idGenero = parameters.Get<byte>("unidGenero");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }
    public void Actualizar(Genero entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Genero 
                   SET Genero = @Nombre,
                       Descripcion = @Descripcion
                   WHERE idGenero = @idGenero";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            Nombre = entidad.Nombre,
            entidad.Descripcion,
            entidad.idGenero
        });
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }
    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        LogQuery("Eliminar", "eliminarGenero", new { unidGenero = id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("eliminarGenero", new { unidGenero = id }, 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }
    public void Eliminar(Genero entidad)
    {
        Eliminar(entidad.idGenero);
    }
    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Genero";
        
        LogQuery("Contar", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("Contar", stopwatch.Elapsed);
        return result;
    }
    public bool Existe(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Genero WHERE idGenero = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }
    // Implementación de IRepositorioBusqueda<Genero>
    public IEnumerable<Genero> BuscarTexto(string termino, params Expression<Func<Genero, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Genero WHERE Genero LIKE @termino OR Descripcion LIKE @termino";
        
        LogQuery("BuscarTexto", sql, new { termino = $"%{termino}%" });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Genero>(sql, new { termino = $"%{termino}%" });
        stopwatch.Stop();
        
        LogExecutionTime("BuscarTexto", stopwatch.Elapsed);
        return result;
    }
    public IEnumerable<Genero> ObtenerPaginado(int pagina, int tamañoPagina, string ordenarPor = "idGenero")
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Genero ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
        
        LogQuery("ObtenerPaginado", sql, new { tamañoPagina, offset });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Genero>(sql, new { tamañoPagina, offset });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaginado", stopwatch.Elapsed);
        return result;
    }
    public IEnumerable<Genero> ObtenerConRelaciones(params Expression<Func<Genero, object>>[] includes)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT g.*, c.*
            FROM Genero g
            LEFT JOIN Cancion c ON g.idGenero = c.idGenero";
        LogQuery("ObtenerConRelaciones", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var generos = new Dictionary<byte, Genero>();
        var result = connection.Query<Genero, Cancion, Genero>(sql,
            (genero, cancion) =>
            {
                if (!generos.TryGetValue(genero.idGenero, out var generoEntry))
                {
                    generoEntry = genero;
                    generoEntry.Canciones = new List<Cancion>();
                    generos.Add(generoEntry.idGenero, generoEntry);
                }
                
                if (cancion != null)
                {
                    generoEntry.Canciones.Add(cancion);
                }
                
                return generoEntry;
            }, splitOn: "idCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelaciones", stopwatch.Elapsed);
        
        return generos.Values;
    }
    // Métodos específicos de IRepositorioGenero
    public IEnumerable<Genero> ObtenerGenerosPopulares()
    {
        using var connection = CreateConnection();
        var sql = @"SELECT g.*, COUNT(c.idCancion) as TotalCanciones
                   FROM Genero g
                   LEFT JOIN Cancion c ON g.idGenero = c.idGenero
                   GROUP BY g.idGenero
                   ORDER BY TotalCanciones DESC";
        LogQuery("ObtenerGenerosPopulares", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Genero>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerGenerosPopulares", stopwatch.Elapsed);
        return result;
    }
    public Genero? ObtenerPorNombre(string nombre)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Genero WHERE Genero = @nombre";
        
        LogQuery("ObtenerPorNombre", sql, new { nombre });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<Genero>(sql, new { nombre });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorNombre", stopwatch.Elapsed);
        return result;
    }

    public async Task<Genero?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
    using var connection = CreateConnection();
    var sql = "SELECT * FROM Genero WHERE idGenero = @id";
        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<Genero>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        return result;
    }
    public async Task<IEnumerable<Genero>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerGeneros");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Genero>(
            new CommandDefinition("ObtenerGeneros", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }
    public async Task<IEnumerable<Genero>> BuscarAsync(Expression<Func<Genero, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }
    public async Task InsertarAsync(Genero entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unGenero", entidad.Nombre);
        parameters.Add("unDescripcion", entidad.Descripcion);
        parameters.Add("unidGenero", dbType: DbType.Byte, direction: ParameterDirection.Output);
        LogQuery("InsertarAsync", "altaGenero", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaGenero", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.idGenero = parameters.Get<byte>("unidGenero");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }
    public async Task ActualizarAsync(Genero entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Genero 
                   SET Genero = @Nombre,
                       Descripcion = @Descripcion
                   WHERE idGenero = @idGenero";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                Nombre = entidad.Nombre,
                entidad.Descripcion,
                entidad.idGenero
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }
    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("EliminarAsync", "eliminarGenero", new { unidGenero = id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("eliminarGenero", new { unidGenero = id }, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }
    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Genero";
        
        LogQuery("ContarAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ContarAsync", stopwatch.Elapsed);
        return result;
    }
    public async Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Genero WHERE idGenero = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }
    public async Task<IEnumerable<Genero>> BuscarTextoAsync(string termino, params Expression<Func<Genero, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Genero WHERE Genero LIKE @termino OR Descripcion LIKE @termino";
        
        LogQuery("BuscarTextoAsync", sql, new { termino = $"%{termino}%" });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Genero>(
            new CommandDefinition(sql, new { termino = $"%{termino}%" }, 
                cancellationToken: CancellationToken.None));
        stopwatch.Stop();
        
        LogExecutionTime("BuscarTextoAsync", stopwatch.Elapsed);
        return result;
    }
    public async Task<IEnumerable<Genero>> ObtenerPaginadoAsync(int pagina, int tamañoPagina, string ordenarPor = "idGenero", CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Genero ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
        
        LogQuery("ObtenerPaginadoAsync", sql, new { tamañoPagina, offset });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Genero>(
            new CommandDefinition(sql, new { tamañoPagina, offset }, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaginadoAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Genero>> ObtenerConRelacionesAsync(params Expression<Func<Genero, object>>[] includes)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT g.*, c.*
            FROM Genero g
            LEFT JOIN Cancion c ON g.idGenero = c.idGenero";
        LogQuery("ObtenerConRelacionesAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var generos = new Dictionary<byte, Genero>();
        var result = await connection.QueryAsync<Genero, Cancion, Genero>(
            new CommandDefinition(sql, cancellationToken: CancellationToken.None),
            (genero, cancion) =>
            {
                if (!generos.TryGetValue(genero.idGenero, out var generoEntry))
                {
                    generoEntry = genero;
                    generoEntry.Canciones = new List<Cancion>();
                    generos.Add(generoEntry.idGenero, generoEntry);
                }
                
                if (cancion != null)
                {
                    generoEntry.Canciones.Add(cancion);
                }
                
                return generoEntry;
            }, splitOn: "idCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelacionesAsync", stopwatch.Elapsed);
        
        return generos.Values;
    }

    public async Task<IEnumerable<Genero>> ObtenerGenerosPopularesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT g.*, COUNT(c.idCancion) as TotalCanciones
                   FROM Genero g
                   LEFT JOIN Cancion c ON g.idGenero = c.idGenero
                   GROUP BY g.idGenero
                   ORDER BY TotalCanciones DESC";

        LogQuery("ObtenerGenerosPopularesAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Genero>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerGenerosPopularesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<Genero?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Genero WHERE Genero = @nombre";
        
        LogQuery("ObtenerPorNombreAsync", sql, new { nombre });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<Genero>(
            new CommandDefinition(sql, new { nombre }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorNombreAsync", stopwatch.Elapsed);
        return result;
    }
}

public class RepoNacionalidad : RepoGenerico, IRepoNacionalidad
{
    public RepoNacionalidad(string connectionString, ILogger<RepoNacionalidad> logger) 
        : base(connectionString, logger) { }

    public Nacionalidad? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Nacionalidad WHERE idNacionalidad = @id";
        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<Nacionalidad>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Nacionalidad> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerNacionalidades");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Nacionalidad>("ObtenerNacionalidades", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Nacionalidad> Buscar(Expression<Func<Nacionalidad, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Nacionalidad entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unPais", entidad.Pais);
        parameters.Add("unidNacionalidad", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaNacionalidad", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaNacionalidad", parameters, commandType: CommandType.StoredProcedure);
        entidad.idNacionalidad = parameters.Get<uint>("unidNacionalidad");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Nacionalidad entidad)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Nacionalidad SET Pais = @Pais WHERE idNacionalidad = @idNacionalidad";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, entidad);
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Nacionalidad WHERE idNacionalidad = @id";
        
        LogQuery("Eliminar", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Nacionalidad entidad)
    {
        Eliminar(entidad.idNacionalidad);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Nacionalidad";
        
        LogQuery("Contar", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("Contar", stopwatch.Elapsed);
        return result;
    }

    public bool Existe(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Nacionalidad WHERE idNacionalidad = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    public Nacionalidad? ObtenerPorPais(string pais)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Nacionalidad WHERE Pais = @pais";
        
        LogQuery("ObtenerPorPais", sql, new { pais });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<Nacionalidad>(sql, new { pais });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorPais", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Nacionalidad> ObtenerPaisesConUsuarios()
    {
        using var connection = CreateConnection();
        var sql = @"SELECT n.*, COUNT(u.idUsuario) as TotalUsuarios
                   FROM Nacionalidad n
                   LEFT JOIN Usuario u ON n.idNacionalidad = u.idNacionalidad
                   GROUP BY n.idNacionalidad
                   HAVING COUNT(u.idUsuario) > 0
                   ORDER BY TotalUsuarios DESC";
        
        LogQuery("ObtenerPaisesConUsuarios", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Nacionalidad>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaisesConUsuarios", stopwatch.Elapsed);
        return result;
    }

    public async Task<Nacionalidad?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Nacionalidad WHERE idNacionalidad = @id";
        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<Nacionalidad>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Nacionalidad>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerNacionalidades");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Nacionalidad>(
            new CommandDefinition("ObtenerNacionalidades", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Nacionalidad>> BuscarAsync(Expression<Func<Nacionalidad, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Nacionalidad entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unPais", entidad.Pais);
        parameters.Add("unidNacionalidad", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaNacionalidad", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaNacionalidad", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.idNacionalidad = parameters.Get<uint>("unidNacionalidad");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Nacionalidad entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Nacionalidad SET Pais = @Pais WHERE idNacionalidad = @idNacionalidad";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, entidad, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Nacionalidad WHERE idNacionalidad = @id";
        
        LogQuery("EliminarAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Nacionalidad";
        
        LogQuery("ContarAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ContarAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Nacionalidad WHERE idNacionalidad = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<Nacionalidad?> ObtenerPorPaisAsync(string pais, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Nacionalidad WHERE Pais = @pais";
        
        LogQuery("ObtenerPorPaisAsync", sql, new { pais });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<Nacionalidad>(
            new CommandDefinition(sql, new { pais }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorPaisAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Nacionalidad>> ObtenerPaisesConUsuariosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT n.*, COUNT(u.idUsuario) as TotalUsuarios
                   FROM Nacionalidad n
                   LEFT JOIN Usuario u ON n.idNacionalidad = u.idNacionalidad
                   GROUP BY n.idNacionalidad
                   HAVING COUNT(u.idUsuario) > 0
                   ORDER BY TotalUsuarios DESC";
        
        LogQuery("ObtenerPaisesConUsuariosAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Nacionalidad>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaisesConUsuariosAsync", stopwatch.Elapsed);
        return result;
    }
}

public class RepoPlaylist : RepoGenerico, IRepoPlaylist
{
    public RepoPlaylist(string connectionString, ILogger<RepoPlaylist> logger) 
        : base(connectionString, logger) { }

    public PlayList? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*, c.*
            FROM Playlist p
            JOIN Usuario u ON p.idUsuario = u.idUsuario
            LEFT JOIN Cancion_Playlist cp ON p.idPlaylist = cp.idPlaylist
            LEFT JOIN Cancion c ON cp.idCancion = c.idCancion
            WHERE p.idPlaylist = @id";

        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var playlists = new Dictionary<uint, PlayList>();
        var result = connection.Query<PlayList, Usuario, Cancion, PlayList>(sql,
            (playlist, usuario, cancion) =>
            {
                if (!playlists.TryGetValue(playlist.idPlaylist, out var playlistEntry))
                {
                    playlistEntry = playlist;
                    playlistEntry.Usuario = usuario;
                    playlistEntry.Canciones = new List<Cancion>();
                    playlists.Add(playlistEntry.idPlaylist, playlistEntry);
                }
                
                if (cancion != null)
                {
                    playlistEntry.Canciones.Add(cancion);
                }
                
                return playlistEntry;
            }, new { id }, splitOn: "idUsuario,idCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        
        return playlists.Values.FirstOrDefault();
    }

    public IEnumerable<PlayList> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerPlayLists");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<PlayList>("ObtenerPlayLists", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<PlayList> Buscar(Expression<Func<PlayList, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(PlayList entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unNombre", entidad.Nombre);
        parameters.Add("unidUsuario", entidad.Usuario.idUsuario);
        parameters.Add("unidPlaylist", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaPlaylist", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaPlaylist", parameters, commandType: CommandType.StoredProcedure);
        entidad.idPlaylist = parameters.Get<uint>("unidPlaylist");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(PlayList entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Playlist 
                   SET Nombre = @Nombre
                   WHERE idPlaylist = @idPlaylist";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            entidad.Nombre,
            entidad.idPlaylist
        });
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Playlist WHERE idPlaylist = @id";
        
        LogQuery("Eliminar", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(PlayList entidad)
    {
        Eliminar(entidad.idPlaylist);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Playlist";
        
        LogQuery("Contar", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("Contar", stopwatch.Elapsed);
        return result;
    }

    public bool Existe(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Playlist WHERE idPlaylist = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<PlayList> ObtenerPorUsuario(uint idUsuario)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*
            FROM Playlist p
            JOIN Usuario u ON p.idUsuario = u.idUsuario
            WHERE p.idUsuario = @idUsuario";
        
        LogQuery("ObtenerPorUsuario", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<PlayList, Usuario, PlayList>(sql,
            (playlist, usuario) =>
            {
                playlist.Usuario = usuario;
                return playlist;
            }, new { idUsuario }, splitOn: "idUsuario");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorUsuario", stopwatch.Elapsed);
        return result;
    }

    public PlayList? ObtenerConCanciones(uint idPlaylist)
    {
        return ObtenerPorId(idPlaylist);
    }

    public bool AgregarCancion(uint idPlaylist, uint idCancion)
    {
        using var connection = CreateConnection();
        LogQuery("AgregarCancion", "altaPlaylistCancion", new { idCancion, idPlaylist });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaPlaylistCancion", new { unidCancion = idCancion, unidPlaylist = idPlaylist }, 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("AgregarCancion", stopwatch.Elapsed);
        return true;
    }

    public bool RemoverCancion(uint idPlaylist, uint idCancion)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist AND idCancion = @idCancion";
        
        LogQuery("RemoverCancion", sql, new { idPlaylist, idCancion });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new { idPlaylist, idCancion });
        stopwatch.Stop();
        
        LogExecutionTime("RemoverCancion", stopwatch.Elapsed);
        return result > 0;
    }

    public bool ReordenarCanciones(uint idPlaylist, List<uint> idsCanciones)
    {
        using var connection = CreateConnection();
        
        // Eliminar todas las canciones actuales
        var deleteSql = "DELETE FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist";
        connection.Execute(deleteSql, new { idPlaylist });
        
        // Insertar en el nuevo orden
        var insertSql = "INSERT INTO Cancion_Playlist (idPlaylist, idCancion, Orden) VALUES (@idPlaylist, @idCancion, @orden)";
        
        var parameters = idsCanciones.Select((idCancion, index) => new 
        { 
            idPlaylist, 
            idCancion, 
            orden = index + 1 
        });
        
        LogQuery("ReordenarCanciones", insertSql, parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(insertSql, parameters);
        stopwatch.Stop();
        
        LogExecutionTime("ReordenarCanciones", stopwatch.Elapsed);
        return result > 0;
    }

    public int ObtenerTotalCanciones(uint idPlaylist)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist";
        
        LogQuery("ObtenerTotalCanciones", sql, new { idPlaylist });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql, new { idPlaylist });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTotalCanciones", stopwatch.Elapsed);
        return result;
    }

    public TimeSpan ObtenerDuracionTotal(uint idPlaylist)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT SEC_TO_TIME(SUM(TIME_TO_SEC(c.Duracion))) 
                   FROM Cancion c
                   JOIN Cancion_Playlist cp ON c.idCancion = cp.idCancion
                   WHERE cp.idPlaylist = @idPlaylist";
        
        LogQuery("ObtenerDuracionTotal", sql, new { idPlaylist });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<TimeSpan?>(sql, new { idPlaylist }) ?? TimeSpan.Zero;
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerDuracionTotal", stopwatch.Elapsed);
        return result;
    }
    
    public async Task<PlayList?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*, c.*
            FROM Playlist p
            JOIN Usuario u ON p.idUsuario = u.idUsuario
            LEFT JOIN Cancion_Playlist cp ON p.idPlaylist = cp.idPlaylist
            LEFT JOIN Cancion c ON cp.idCancion = c.idCancion
            WHERE p.idPlaylist = @id";
    
        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var playlists = new Dictionary<uint, PlayList>();
        var result = await connection.QueryAsync<PlayList, Usuario, Cancion, PlayList>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (playlist, usuario, cancion) =>
            {
                if (!playlists.TryGetValue(playlist.idPlaylist, out var playlistEntry))
                {
                    playlistEntry = playlist;
                    playlistEntry.Usuario = usuario;
                    playlistEntry.Canciones = new List<Cancion>();
                    playlists.Add(playlistEntry.idPlaylist, playlistEntry);
                }
                
                if (cancion != null)
                {
                    playlistEntry.Canciones.Add(cancion);
                }
                
                return playlistEntry;
            }, splitOn: "idUsuario,idCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        
        return playlists.Values.FirstOrDefault();
    }

    public async Task<IEnumerable<PlayList>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerPlayLists");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<PlayList>(
            new CommandDefinition("ObtenerPlayLists", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<PlayList>> BuscarAsync(Expression<Func<PlayList, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(PlayList entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unNombre", entidad.Nombre);
        parameters.Add("unidUsuario", entidad.Usuario.idUsuario);
        parameters.Add("unidPlaylist", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaPlaylist", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaPlaylist", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.idPlaylist = parameters.Get<uint>("unidPlaylist");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(PlayList entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Playlist 
                   SET Nombre = @Nombre
                   WHERE idPlaylist = @idPlaylist";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                entidad.Nombre,
                entidad.idPlaylist
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Playlist WHERE idPlaylist = @id";
        
        LogQuery("EliminarAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Playlist";
        
        LogQuery("ContarAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ContarAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Playlist WHERE idPlaylist = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<PlayList>> ObtenerPorUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*
            FROM Playlist p
            JOIN Usuario u ON p.idUsuario = u.idUsuario
            WHERE p.idUsuario = @idUsuario";
    
        LogQuery("ObtenerPorUsuarioAsync", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<PlayList, Usuario, PlayList>(
            new CommandDefinition(sql, new { idUsuario }, cancellationToken: cancellationToken),
            (playlist, usuario) =>
            {
                playlist.Usuario = usuario;
                return playlist;
            }, splitOn: "idUsuario");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorUsuarioAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<PlayList?> ObtenerConCancionesAsync(uint idPlaylist, CancellationToken cancellationToken = default)
    {
        return await ObtenerPorIdAsync(idPlaylist, cancellationToken);
    }

    public async Task<bool> AgregarCancionAsync(uint idPlaylist, uint idCancion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("AgregarCancionAsync", "altaPlaylistCancion", new { idCancion, idPlaylist });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaPlaylistCancion", new { unidCancion = idCancion, unidPlaylist = idPlaylist }, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("AgregarCancionAsync", stopwatch.Elapsed);
        return true;
    }

    public async Task<bool> RemoverCancionAsync(uint idPlaylist, uint idCancion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist AND idCancion = @idCancion";
        
        LogQuery("RemoverCancionAsync", sql, new { idPlaylist, idCancion });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { idPlaylist, idCancion }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("RemoverCancionAsync", stopwatch.Elapsed);
        return result > 0;
    }

    public async Task<bool> ReordenarCancionesAsync(uint idPlaylist, List<uint> idsCanciones, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        
        // Eliminar todas las canciones actuales
        var deleteSql = "DELETE FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist";
        await connection.ExecuteAsync(new CommandDefinition(deleteSql, new { idPlaylist }, cancellationToken: cancellationToken));
        
        // Insertar en el nuevo orden
        var insertSql = "INSERT INTO Cancion_Playlist (idPlaylist, idCancion, Orden) VALUES (@idPlaylist, @idCancion, @orden)";
        
        var parameters = idsCanciones.Select((idCancion, index) => new 
        { 
            idPlaylist, 
            idCancion, 
            orden = index + 1 
        });
        
        LogQuery("ReordenarCancionesAsync", insertSql, parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(insertSql, parameters, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ReordenarCancionesAsync", stopwatch.Elapsed);
        return result > 0;
    }

    public async Task<int> ObtenerTotalCancionesAsync(uint idPlaylist, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist";
        
        LogQuery("ObtenerTotalCancionesAsync", sql, new { idPlaylist });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { idPlaylist }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTotalCancionesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<TimeSpan> ObtenerDuracionTotalAsync(uint idPlaylist, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT SEC_TO_TIME(SUM(TIME_TO_SEC(c.Duracion))) 
                   FROM Cancion c
                   JOIN Cancion_Playlist cp ON c.idCancion = cp.idCancion
                   WHERE cp.idPlaylist = @idPlaylist";
        
        LogQuery("ObtenerDuracionTotalAsync", sql, new { idPlaylist });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<TimeSpan?>(
            new CommandDefinition(sql, new { idPlaylist }, cancellationToken: cancellationToken)) ?? TimeSpan.Zero;
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerDuracionTotalAsync", stopwatch.Elapsed);
        return result;
    }
}

public class RepoReproduccion : RepoGenerico, IRepoReproduccion
{
    public RepoReproduccion(string connectionString, ILogger<RepoReproduccion> logger) 
    : base(connectionString, logger) { }

    public Reproduccion? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, c.*
            FROM HistorialReproduccion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN Cancion c ON r.idCancion = c.idCancion
            WHERE r.idHistorial = @id";
        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Reproduccion, Usuario, Cancion, Reproduccion>(sql,
            (reproduccion, usuario, cancion) =>
            {
                reproduccion.Usuario = usuario;
                reproduccion.Cancion = cancion;
                return reproduccion;
            }, new { id }, splitOn: "idUsuario,idCancion").FirstOrDefault();
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Reproduccion> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerHistorialReproduccion");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Reproduccion>("ObtenerHistorialReproduccion", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Reproduccion> Buscar(Expression<Func<Reproduccion, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Reproduccion entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unidUsuario", entidad.Usuario.idUsuario);
        parameters.Add("unidCancion", entidad.Cancion.idCancion);
        parameters.Add("unFechaReproduccion", entidad.FechaReproduccion);
        parameters.Add("unidHistorial", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaHistorial_reproduccion", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaHistorial_reproduccion", parameters, commandType: CommandType.StoredProcedure);
        entidad.IdHistorial = parameters.Get<uint>("unidHistorial");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Reproduccion entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE HistorialReproduccion 
                   SET idUsuario = @idUsuario,
                       idCancion = @idCancion,
                       FechaReproduccion = @FechaReproduccion,
                       ProgresoReproduccion = @ProgresoReproduccion,
                       ReproduccionCompleta = @ReproduccionCompleta,
                       Dispositivo = @Dispositivo
                   WHERE idHistorial = @IdHistorial";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            idUsuario = entidad.Usuario.idUsuario,
            idCancion = entidad.Cancion.idCancion,
            entidad.FechaReproduccion,
            entidad.ProgresoReproduccion,
            entidad.ReproduccionCompleta,
            entidad.Dispositivo,
            entidad.IdHistorial
        });
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM HistorialReproduccion WHERE idHistorial = @id";
        
        LogQuery("Eliminar", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Reproduccion entidad)
    {
        Eliminar(entidad.IdHistorial);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM HistorialReproduccion";
        
        LogQuery("Contar", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("Contar", stopwatch.Elapsed);
        return result;
    }

    public bool Existe(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM HistorialReproduccion WHERE idHistorial = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Reproduccion> ObtenerHistorialUsuario(uint idUsuario, int limite = 50)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, c.*
            FROM HistorialReproduccion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN Cancion c ON r.idCancion = c.idCancion
            WHERE r.idUsuario = @idUsuario
            ORDER BY r.FechaReproduccion DESC
            LIMIT @limite";
        
        LogQuery("ObtenerHistorialUsuario", sql, new { idUsuario, limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Reproduccion, Usuario, Cancion, Reproduccion>(sql,
            (reproduccion, usuario, cancion) =>
            {
                reproduccion.Usuario = usuario;
                reproduccion.Cancion = cancion;
                return reproduccion;
            }, new { idUsuario, limite }, splitOn: "idUsuario,idCancion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerHistorialUsuario", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Reproduccion> ObtenerRecientes(DateTime desde)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, c.*
            FROM HistorialReproduccion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN Cancion c ON r.idCancion = c.idCancion
            WHERE r.FechaReproduccion >= @desde
            ORDER BY r.FechaReproduccion DESC";

        LogQuery("ObtenerRecientes", sql, new { desde });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Reproduccion, Usuario, Cancion, Reproduccion>(sql,
            (reproduccion, usuario, cancion) =>
            {
                reproduccion.Usuario = usuario;
                reproduccion.Cancion = cancion;
                return reproduccion;
            }, new { desde }, splitOn: "idUsuario,idCancion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerRecientes", stopwatch.Elapsed);
        return result;
    }

    public bool RegistrarReproduccion(uint idUsuario, uint idCancion, TimeSpan progreso, bool completa, string? dispositivo)
    {
        using var connection = CreateConnection();
        var sql = @"INSERT INTO HistorialReproduccion (idUsuario, idCancion, FechaReproduccion, ProgresoReproduccion, ReproduccionCompleta, Dispositivo)
                   VALUES (@idUsuario, @idCancion, NOW(), @progreso, @completa, @dispositivo)";
        
        LogQuery("RegistrarReproduccion", sql, new { idUsuario, idCancion, progreso, completa, dispositivo });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new 
        { 
            idUsuario, 
            idCancion, 
            progreso, 
            completa, 
            dispositivo 
        });
        stopwatch.Stop();
        
        LogExecutionTime("RegistrarReproduccion", stopwatch.Elapsed);
        return result > 0;
    }

    public IEnumerable<Cancion> ObtenerCancionesMasEscuchadas(uint idUsuario, int limite = 10)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT c.*, COUNT(r.idHistorial) as TotalReproducciones
                   FROM Cancion c
                   JOIN HistorialReproduccion r ON c.idCancion = r.idCancion
                   WHERE r.idUsuario = @idUsuario
                   GROUP BY c.idCancion
                   ORDER BY TotalReproducciones DESC
                   LIMIT @limite";
        
        LogQuery("ObtenerCancionesMasEscuchadas", sql, new { idUsuario, limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>(sql, new { idUsuario, limite });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerCancionesMasEscuchadas", stopwatch.Elapsed);
        return result;
    }

    public int ObtenerTotalReproducciones(uint idUsuario)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM HistorialReproduccion WHERE idUsuario = @idUsuario";
        
        LogQuery("ObtenerTotalReproducciones", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql, new { idUsuario });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTotalReproducciones", stopwatch.Elapsed);
        return result;
    }

    public TimeSpan ObtenerTiempoTotalEscuchado(uint idUsuario)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT SEC_TO_TIME(SUM(TIME_TO_SEC(c.Duracion))) 
                   FROM Cancion c
                   JOIN HistorialReproduccion r ON c.idCancion = r.idCancion
                   WHERE r.idUsuario = @idUsuario AND r.ReproduccionCompleta = 1";
        
        LogQuery("ObtenerTiempoTotalEscuchado", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<TimeSpan?>(sql, new { idUsuario }) ?? TimeSpan.Zero;
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTiempoTotalEscuchado", stopwatch.Elapsed);
        return result;
    }

    public async Task<Reproduccion?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, c.*
            FROM HistorialReproduccion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN Cancion c ON r.idCancion = c.idCancion
            WHERE r.idHistorial = @id";

        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Reproduccion, Usuario, Cancion, Reproduccion>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (reproduccion, usuario, cancion) =>
            {
                reproduccion.Usuario = usuario;
                reproduccion.Cancion = cancion;
                return reproduccion;
            }, splitOn: "idUsuario,idCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Reproduccion>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerHistorialReproduccion");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Reproduccion>(
            new CommandDefinition("ObtenerHistorialReproduccion", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Reproduccion>> BuscarAsync(Expression<Func<Reproduccion, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Reproduccion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unidUsuario", entidad.Usuario.idUsuario);
        parameters.Add("unidCancion", entidad.Cancion.idCancion);
        parameters.Add("unFechaReproduccion", entidad.FechaReproduccion);
        parameters.Add("unidHistorial", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaHistorial_reproduccion", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaHistorial_reproduccion", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.IdHistorial = parameters.Get<uint>("unidHistorial");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Reproduccion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE HistorialReproduccion 
                   SET idUsuario = @idUsuario,
                       idCancion = @idCancion,
                       FechaReproduccion = @FechaReproduccion,
                       ProgresoReproduccion = @ProgresoReproduccion,
                       ReproduccionCompleta = @ReproduccionCompleta,
                       Dispositivo = @Dispositivo
                   WHERE idHistorial = @IdHistorial";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                idUsuario = entidad.Usuario.idUsuario,
                idCancion = entidad.Cancion.idCancion,
                entidad.FechaReproduccion,
                entidad.ProgresoReproduccion,
                entidad.ReproduccionCompleta,
                entidad.Dispositivo,
                entidad.IdHistorial
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM HistorialReproduccion WHERE idHistorial = @id";
        
        LogQuery("EliminarAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM HistorialReproduccion";
        
        LogQuery("ContarAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ContarAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM HistorialReproduccion WHERE idHistorial = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Reproduccion>> ObtenerHistorialUsuarioAsync(uint idUsuario, int limite = 50, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, c.*
            FROM HistorialReproduccion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN Cancion c ON r.idCancion = c.idCancion
            WHERE r.idUsuario = @idUsuario
            ORDER BY r.FechaReproduccion DESC
            LIMIT @limite";

        LogQuery("ObtenerHistorialUsuarioAsync", sql, new { idUsuario, limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Reproduccion, Usuario, Cancion, Reproduccion>(
            new CommandDefinition(sql, new { idUsuario, limite }, cancellationToken: cancellationToken),
            (reproduccion, usuario, cancion) =>
            {
                reproduccion.Usuario = usuario;
                reproduccion.Cancion = cancion;
                return reproduccion;
            }, splitOn: "idUsuario,idCancion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerHistorialUsuarioAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Reproduccion>> ObtenerRecientesAsync(DateTime desde, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, c.*
            FROM HistorialReproduccion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN Cancion c ON r.idCancion = c.idCancion
            WHERE r.FechaReproduccion >= @desde
            ORDER BY r.FechaReproduccion DESC";

        LogQuery("ObtenerRecientesAsync", sql, new { desde });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Reproduccion, Usuario, Cancion, Reproduccion>(
            new CommandDefinition(sql, new { desde }, cancellationToken: cancellationToken),
            (reproduccion, usuario, cancion) =>
            {
                reproduccion.Usuario = usuario;
                reproduccion.Cancion = cancion;
                return reproduccion;
            }, splitOn: "idUsuario,idCancion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerRecientesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> RegistrarReproduccionAsync(uint idUsuario, uint idCancion, TimeSpan progreso, bool completa, string? dispositivo, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"INSERT INTO HistorialReproduccion (idUsuario, idCancion, FechaReproduccion, ProgresoReproduccion, ReproduccionCompleta, Dispositivo)
                   VALUES (@idUsuario, @idCancion, NOW(), @progreso, @completa, @dispositivo)";
        
        LogQuery("RegistrarReproduccionAsync", sql, new { idUsuario, idCancion, progreso, completa, dispositivo });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            { 
                idUsuario, 
                idCancion, 
                progreso, 
                completa, 
                dispositivo 
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("RegistrarReproduccionAsync", stopwatch.Elapsed);
        return result > 0;
    }

    public async Task<IEnumerable<Cancion>> ObtenerCancionesMasEscuchadasAsync(uint idUsuario, int limite = 10, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT c.*, COUNT(r.idHistorial) as TotalReproducciones
                   FROM Cancion c
                   JOIN HistorialReproduccion r ON c.idCancion = r.idCancion
                   WHERE r.idUsuario = @idUsuario
                   GROUP BY c.idCancion
                   ORDER BY TotalReproducciones DESC
                   LIMIT @limite";
        
        LogQuery("ObtenerCancionesMasEscuchadasAsync", sql, new { idUsuario, limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion>(
            new CommandDefinition(sql, new { idUsuario, limite }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerCancionesMasEscuchadasAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<int> ObtenerTotalReproduccionesAsync(uint idUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM HistorialReproduccion WHERE idUsuario = @idUsuario";
        
        LogQuery("ObtenerTotalReproduccionesAsync", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { idUsuario }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTotalReproduccionesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<TimeSpan> ObtenerTiempoTotalEscuchadoAsync(uint idUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT SEC_TO_TIME(SUM(TIME_TO_SEC(c.Duracion))) 
                   FROM Cancion c
                   JOIN HistorialReproduccion r ON c.idCancion = r.idCancion
                   WHERE r.idUsuario = @idUsuario AND r.ReproduccionCompleta = 1";
        
        LogQuery("ObtenerTiempoTotalEscuchadoAsync", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<TimeSpan?>(
            new CommandDefinition(sql, new { idUsuario }, cancellationToken: cancellationToken)) ?? TimeSpan.Zero;
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTiempoTotalEscuchadoAsync", stopwatch.Elapsed);
        return result;
    }
}

public class RepoSuscripcion : RepoGenerico, IRepoRegistro
{
    public RepoSuscripcion(string connectionString, ILogger<RepoSuscripcion> logger) 
        : base(connectionString, logger) { }

    public Registro? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE r.idSuscripcion = @id";

        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Registro, Usuario, TipoSuscripcion, Registro>(sql,
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, new { id }, splitOn: "idUsuario,idTipoSuscripcion").FirstOrDefault();
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Registro> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerSuscripciones");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Registro>("ObtenerSuscripciones", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Registro> Buscar(Expression<Func<Registro, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Registro entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unIdUsuario", entidad.Usuario.idUsuario);
        parameters.Add("unidTipoSuscripcion", entidad.TipoSuscripcion.IdTipoSuscripcion);
        parameters.Add("unidSuscripcion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaRegistroSuscripcion", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaRegistroSuscripcion", parameters, commandType: CommandType.StoredProcedure);
        entidad.IdSuscripcion = parameters.Get<uint>("unidSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Registro entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Suscripcion 
                   SET idUsuario = @idUsuario,
                       idTipoSuscripcion = @idTipoSuscripcion,
                       FechaInicio = @FechaInicio,
                       FechaRenovacion = @FechaRenovacion,
                       AutoRenovacion = @AutoRenovacion,
                       MetodoPago = @MetodoPago
                   WHERE idSuscripcion = @IdSuscripcion";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            idUsuario = entidad.Usuario.idUsuario,
            idTipoSuscripcion = entidad.TipoSuscripcion.IdTipoSuscripcion,
            entidad.FechaInicio,
            entidad.FechaRenovacion,
            entidad.AutoRenovacion,
            entidad.MetodoPago,
            entidad.IdSuscripcion
        });
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Suscripcion WHERE idSuscripcion = @id";
        
        LogQuery("Eliminar", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Registro entidad)
    {
        Eliminar(entidad.IdSuscripcion);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Suscripcion";
        
        LogQuery("Contar", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("Contar", stopwatch.Elapsed);
        return result;
    }

    public bool Existe(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Suscripcion WHERE idSuscripcion = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    public Registro? ObtenerSuscripcionActiva(uint idUsuario)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE r.idUsuario = @idUsuario
            AND DATE_ADD(r.FechaInicio, INTERVAL ts.Duracion MONTH) >= CURDATE()
            ORDER BY r.FechaInicio DESC
            LIMIT 1";

        LogQuery("ObtenerSuscripcionActiva", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Registro, Usuario, TipoSuscripcion, Registro>(sql,
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, new { idUsuario }, splitOn: "idUsuario,idTipoSuscripcion").FirstOrDefault();
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionActiva", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Registro> ObtenerSuscripcionesPorUsuario(uint idUsuario)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE r.idUsuario = @idUsuario
            ORDER BY r.FechaInicio DESC";

        LogQuery("ObtenerSuscripcionesPorUsuario", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Registro, Usuario, TipoSuscripcion, Registro>(sql,
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, new { idUsuario }, splitOn: "idUsuario,idTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionesPorUsuario", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Registro> ObtenerSuscripcionesExpiradas()
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE DATE_ADD(r.FechaInicio, INTERVAL ts.Duracion MONTH) < CURDATE()";

        LogQuery("ObtenerSuscripcionesExpiradas", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Registro, Usuario, TipoSuscripcion, Registro>(sql,
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, splitOn: "idUsuario,idTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionesExpiradas", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Registro> ObtenerSuscripcionesPorExpirar(int dias = 7)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE DATE_ADD(r.FechaInicio, INTERVAL ts.Duracion MONTH) 
                  BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL @dias DAY)";

        LogQuery("ObtenerSuscripcionesPorExpirar", sql, new { dias });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Registro, Usuario, TipoSuscripcion, Registro>(sql,
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, new { dias }, splitOn: "idUsuario,idTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionesPorExpirar", stopwatch.Elapsed);
        return result;
    }

    public bool RenovarSuscripcion(uint idSuscripcion)
    {
        using var connection = CreateConnection();
        const string sql = @"
            UPDATE Suscripcion 
            SET FechaInicio = CURDATE(),
                FechaRenovacion = CURDATE()
            WHERE idSuscripcion = @idSuscripcion";

        LogQuery("RenovarSuscripcion", sql, new { idSuscripcion });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new { idSuscripcion });
        stopwatch.Stop();
        
        LogExecutionTime("RenovarSuscripcion", stopwatch.Elapsed);
        return result > 0;
    }

    public bool CancelarSuscripcion(uint idSuscripcion)
    {
        using var connection = CreateConnection();
        const string sql = @"
            UPDATE Suscripcion 
            SET AutoRenovacion = 0
            WHERE idSuscripcion = @idSuscripcion";

        LogQuery("CancelarSuscripcion", sql, new { idSuscripcion });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new { idSuscripcion });
        stopwatch.Stop();
        
        LogExecutionTime("CancelarSuscripcion", stopwatch.Elapsed);
        return result > 0;
    }
    
    public async Task<Registro?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE r.idSuscripcion = @id";

        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Registro, Usuario, TipoSuscripcion, Registro>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, splitOn: "idUsuario,idTipoSuscripcion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Registro>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerSuscripciones");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Registro>(
            new CommandDefinition("ObtenerSuscripciones", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Registro>> BuscarAsync(Expression<Func<Registro, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Registro entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unIdUsuario", entidad.Usuario.idUsuario);
        parameters.Add("unidTipoSuscripcion", entidad.TipoSuscripcion.IdTipoSuscripcion);
        parameters.Add("unidSuscripcion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaRegistroSuscripcion", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaRegistroSuscripcion", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.IdSuscripcion = parameters.Get<uint>("unidSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Registro entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Suscripcion 
                   SET idUsuario = @idUsuario,
                       idTipoSuscripcion = @idTipoSuscripcion,
                       FechaInicio = @FechaInicio,
                       FechaRenovacion = @FechaRenovacion,
                       AutoRenovacion = @AutoRenovacion,
                       MetodoPago = @MetodoPago
                   WHERE idSuscripcion = @IdSuscripcion";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                idUsuario = entidad.Usuario.idUsuario,
                idTipoSuscripcion = entidad.TipoSuscripcion.IdTipoSuscripcion,
                entidad.FechaInicio,
                entidad.FechaRenovacion,
                entidad.AutoRenovacion,
                entidad.MetodoPago,
                entidad.IdSuscripcion
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Suscripcion WHERE idSuscripcion = @id";
        
        LogQuery("EliminarAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Suscripcion";
        
        LogQuery("ContarAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ContarAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Suscripcion WHERE idSuscripcion = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<Registro?> ObtenerSuscripcionActivaAsync(uint idUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE r.idUsuario = @idUsuario
            AND DATE_ADD(r.FechaInicio, INTERVAL ts.Duracion MONTH) >= CURDATE()
            ORDER BY r.FechaInicio DESC
            LIMIT 1";
    
        LogQuery("ObtenerSuscripcionActivaAsync", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Registro, Usuario, TipoSuscripcion, Registro>(
            new CommandDefinition(sql, new { idUsuario }, cancellationToken: cancellationToken),
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, splitOn: "idUsuario,idTipoSuscripcion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerSuscripcionActivaAsync", stopwatch.Elapsed);
        
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Registro>> ObtenerSuscripcionesPorUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE r.idUsuario = @idUsuario
            ORDER BY r.FechaInicio DESC";

        LogQuery("ObtenerSuscripcionesPorUsuarioAsync", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Registro, Usuario, TipoSuscripcion, Registro>(
            new CommandDefinition(sql, new { idUsuario }, cancellationToken: cancellationToken),
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, splitOn: "idUsuario,idTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionesPorUsuarioAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Registro>> ObtenerSuscripcionesExpiradasAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE DATE_ADD(r.FechaInicio, INTERVAL ts.Duracion MONTH) < CURDATE()";

        LogQuery("ObtenerSuscripcionesExpiradasAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Registro, Usuario, TipoSuscripcion, Registro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken),
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, splitOn: "idUsuario,idTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionesExpiradasAsync", stopwatch.Elapsed);
        return result;
    }
    
    public async Task<IEnumerable<Registro>> ObtenerSuscripcionesPorExpirarAsync(int dias = 7, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE DATE_ADD(r.FechaInicio, INTERVAL ts.Duracion MONTH) 
                  BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL @dias DAY)";

        LogQuery("ObtenerSuscripcionesPorExpirarAsync", sql, new { dias });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Registro, Usuario, TipoSuscripcion, Registro>(
            new CommandDefinition(sql, new { dias }, cancellationToken: cancellationToken),
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, splitOn: "idUsuario,idTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionesPorExpirarAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> RenovarSuscripcionAsync(uint idSuscripcion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            UPDATE Suscripcion 
            SET FechaInicio = CURDATE(),
                FechaRenovacion = CURDATE()
            WHERE idSuscripcion = @idSuscripcion";

        LogQuery("RenovarSuscripcionAsync", sql, new { idSuscripcion });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { idSuscripcion }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("RenovarSuscripcionAsync", stopwatch.Elapsed);
        return result > 0;
    }

    public async Task<bool> CancelarSuscripcionAsync(uint idSuscripcion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            UPDATE Suscripcion 
            SET AutoRenovacion = 0
            WHERE idSuscripcion = @idSuscripcion";
            
        LogQuery("CancelarSuscripcionAsync", sql, new { idSuscripcion });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { idSuscripcion }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("CancelarSuscripcionAsync", stopwatch.Elapsed);
        return result > 0;
    }
}

public class RepoTipoSuscripcion : RepoGenerico, IRepoTipoSuscripcion
{
    public RepoTipoSuscripcion(string connectionString, ILogger<RepoTipoSuscripcion> logger) 
        : base(connectionString, logger) { }

    public TipoSuscripcion? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM TipoSuscripcion WHERE idTipoSuscripcion = @id";
        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<TipoSuscripcion>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }
    
    public IEnumerable<TipoSuscripcion> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerTipoSuscripciones");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<TipoSuscripcion>("ObtenerTipoSuscripciones", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }
    
    public IEnumerable<TipoSuscripcion> Buscar(Expression<Func<TipoSuscripcion, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(TipoSuscripcion entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unaDuracion", entidad.Duracion);
        parameters.Add("unCosto", entidad.Costo);
        parameters.Add("UntipoSuscripcion", entidad.Tipo);
        parameters.Add("unidTipoSuscripcion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaTipoSuscripcion", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaTipoSuscripcion", parameters, commandType: CommandType.StoredProcedure);
        entidad.IdTipoSuscripcion = parameters.Get<uint>("unidTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }
    
    public void Actualizar(TipoSuscripcion entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE TipoSuscripcion 
                   SET Duracion = @Duracion,
                       Costo = @Costo,
                       Tipo = @Tipo
                   WHERE idTipoSuscripcion = @IdTipoSuscripcion";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, entidad);
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM TipoSuscripcion WHERE idTipoSuscripcion = @id";
        
        LogQuery("Eliminar", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }
    
    public void Eliminar(TipoSuscripcion entidad)
    {
        Eliminar(entidad.IdTipoSuscripcion);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM TipoSuscripcion";
        
        LogQuery("Contar", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("Contar", stopwatch.Elapsed);
        return result;
    }
    
    public bool Existe(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM TipoSuscripcion WHERE idTipoSuscripcion = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    public TipoSuscripcion? ObtenerMasPopular()
    {
        using var connection = CreateConnection();
        var sql = @"SELECT ts.*, COUNT(s.idSuscripcion) as TotalSuscripciones
                   FROM TipoSuscripcion ts
                   LEFT JOIN Suscripcion s ON ts.idTipoSuscripcion = s.idTipoSuscripcion
                   GROUP BY ts.idTipoSuscripcion
                   ORDER BY TotalSuscripciones DESC
                   LIMIT 1";
        
        LogQuery("ObtenerMasPopular", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<TipoSuscripcion>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerMasPopular", stopwatch.Elapsed);
        return result;
    }
    
    public IEnumerable<TipoSuscripcion> ObtenerOrdenadosPorPrecio()
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM TipoSuscripcion ORDER BY Costo ASC";
        
        LogQuery("ObtenerOrdenadosPorPrecio", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<TipoSuscripcion>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerOrdenadosPorPrecio", stopwatch.Elapsed);
        return result;
    }

    public async Task<TipoSuscripcion?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM TipoSuscripcion WHERE idTipoSuscripcion = @id";
        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<TipoSuscripcion>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        return result;
    }
    
    public async Task<IEnumerable<TipoSuscripcion>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerTipoSuscripciones");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<TipoSuscripcion>(
            new CommandDefinition("ObtenerTipoSuscripciones", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<TipoSuscripcion>> BuscarAsync(Expression<Func<TipoSuscripcion, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }
    
    public async Task InsertarAsync(TipoSuscripcion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unaDuracion", entidad.Duracion);
        parameters.Add("unCosto", entidad.Costo);
        parameters.Add("UntipoSuscripcion", entidad.Tipo);
        parameters.Add("unidTipoSuscripcion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaTipoSuscripcion", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaTipoSuscripcion", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.IdTipoSuscripcion = parameters.Get<uint>("unidTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(TipoSuscripcion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE TipoSuscripcion 
                   SET Duracion = @Duracion,
                       Costo = @Costo,
                       Tipo = @Tipo
                   WHERE idTipoSuscripcion = @IdTipoSuscripcion";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, entidad, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM TipoSuscripcion WHERE idTipoSuscripcion = @id";
        
        LogQuery("EliminarAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM TipoSuscripcion";
        
        LogQuery("ContarAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ContarAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM TipoSuscripcion WHERE idTipoSuscripcion = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<TipoSuscripcion?> ObtenerMasPopularAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT ts.*, COUNT(s.idSuscripcion) as TotalSuscripciones
                   FROM TipoSuscripcion ts
                   LEFT JOIN Suscripcion s ON ts.idTipoSuscripcion = s.idTipoSuscripcion
                   GROUP BY ts.idTipoSuscripcion
                   ORDER BY TotalSuscripciones DESC
                   LIMIT 1";
        
        LogQuery("ObtenerMasPopularAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<TipoSuscripcion>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerMasPopularAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<TipoSuscripcion>> ObtenerOrdenadosPorPrecioAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM TipoSuscripcion ORDER BY Costo ASC";
        
        LogQuery("ObtenerOrdenadosPorPrecioAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<TipoSuscripcion>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerOrdenadosPorPrecioAsync", stopwatch.Elapsed);
        return result;
    }
}

public class RepoUsuario : RepoGenerico, IRepoUsuario
{
    public RepoUsuario(string connectionString, ILogger<RepoUsuario> logger) 
        : base(connectionString, logger) { }

    public Usuario? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.idNacionalidad = n.idNacionalidad
            WHERE u.idUsuario = @id";
        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Usuario, Nacionalidad, Usuario>(sql,
            (usuario, nacionalidad) =>
            {
                usuario.Nacionalidad = nacionalidad;
                return usuario;
            }, new { id }, splitOn: "idNacionalidad").FirstOrDefault();
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Usuario> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerUsuarios");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Usuario>("ObtenerUsuarios", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Usuario> Buscar(Expression<Func<Usuario, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Usuario entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unNombreUsuario", entidad.NombreUsuario);
        parameters.Add("unEmail", entidad.Email);
        parameters.Add("unaContrasenia", entidad.Contrasenia);
        parameters.Add("unidNacionalidad", entidad.Nacionalidad.idNacionalidad);
        parameters.Add("unidUsuario", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaUsuario", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaUsuario", parameters, commandType: CommandType.StoredProcedure);
        entidad.idUsuario = parameters.Get<uint>("unidUsuario");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Usuario entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Usuario 
                   SET NombreUsuario = @NombreUsuario,
                       Email = @Email,
                       Contrasenia = @Contrasenia,
                       idNacionalidad = @idNacionalidad
                   WHERE idUsuario = @idUsuario";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            entidad.NombreUsuario,
            entidad.Email,
            entidad.Contrasenia,
            idNacionalidad = entidad.Nacionalidad.idNacionalidad,
            entidad.idUsuario
        });
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Usuario WHERE idUsuario = @id";
        
        LogQuery("Eliminar", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Usuario entidad)
    {
        Eliminar(entidad.idUsuario);
    }
    
    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Usuario";
        
        LogQuery("Contar", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("Contar", stopwatch.Elapsed);
        return result;
    }

    public bool Existe(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Usuario WHERE idUsuario = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.idNacionalidad = n.idNacionalidad
            WHERE u.Email = @email";

        LogQuery("ObtenerPorEmail", sql, new { email });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Usuario, Nacionalidad, Usuario>(sql,
            (usuario, nacionalidad) =>
            {
                usuario.Nacionalidad = nacionalidad;
                return usuario;
            }, new { email }, splitOn: "idNacionalidad").FirstOrDefault();
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorEmail", stopwatch.Elapsed);
        return result;
    }

    public Usuario? ObtenerPorNombreUsuario(string nombreUsuario)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.idNacionalidad = n.idNacionalidad
            WHERE u.NombreUsuario = @nombreUsuario";

        LogQuery("ObtenerPorNombreUsuario", sql, new { nombreUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Usuario, Nacionalidad, Usuario>(sql,
            (usuario, nacionalidad) =>
            {
                usuario.Nacionalidad = nacionalidad;
                return usuario;
            }, new { nombreUsuario }, splitOn: "idNacionalidad").FirstOrDefault();
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorNombreUsuario", stopwatch.Elapsed);
        return result;
    }

    public Usuario? ObtenerConPlaylists(uint idUsuario)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*, p.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.idNacionalidad = n.idNacionalidad
            LEFT JOIN Playlist p ON u.idUsuario = p.idUsuario
            WHERE u.idUsuario = @idUsuario";

        LogQuery("ObtenerConPlaylists", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var usuarios = new Dictionary<uint, Usuario>();
        var result = connection.Query<Usuario, Nacionalidad, PlayList, Usuario>(sql,
            (usuario, nacionalidad, playlist) =>
            {
                if (!usuarios.TryGetValue(usuario.idUsuario, out var usuarioEntry))
                {
                    usuarioEntry = usuario;
                    usuarioEntry.Nacionalidad = nacionalidad;
                    usuarioEntry.Playlists = new List<PlayList>();
                    usuarios.Add(usuarioEntry.idUsuario, usuarioEntry);
                }
                
                if (playlist != null)
                {
                    usuarioEntry.Playlists.Add(playlist);
                }
                
                return usuarioEntry;
            }, new { idUsuario }, splitOn: "idNacionalidad,idPlaylist");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConPlaylists", stopwatch.Elapsed);
        
        return usuarios.Values.FirstOrDefault();
    }

    public Usuario? ObtenerConSuscripciones(uint idUsuario)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*, r.*, ts.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.idNacionalidad = n.idNacionalidad
            LEFT JOIN Suscripcion r ON u.idUsuario = r.idUsuario
            LEFT JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE u.idUsuario = @idUsuario";

        LogQuery("ObtenerConSuscripciones", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var usuarios = new Dictionary<uint, Usuario>();
        var result = connection.Query<Usuario, Nacionalidad, Registro, TipoSuscripcion, Usuario>(sql,
            (usuario, nacionalidad, registro, tipoSuscripcion) =>
            {
                if (!usuarios.TryGetValue(usuario.idUsuario, out var usuarioEntry))
                {
                    usuarioEntry = usuario;
                    usuarioEntry.Nacionalidad = nacionalidad;
                    usuarioEntry.Suscripciones = new List<Registro>();
                    usuarios.Add(usuarioEntry.idUsuario, usuarioEntry);
                }
                
                if (registro != null)
                {
                    registro.TipoSuscripcion = tipoSuscripcion;
                    usuarioEntry.Suscripciones.Add(registro);
                }
                
                return usuarioEntry;
            }, new { idUsuario }, splitOn: "idNacionalidad,idSuscripcion,idTipoSuscripcion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConSuscripciones", stopwatch.Elapsed);
        
        return usuarios.Values.FirstOrDefault();
    }

    public bool VerificarCredenciales(string email, string contraseña)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Usuario WHERE Email = @email AND Contrasenia = SHA2(@contraseña, 256)";
        
        LogQuery("VerificarCredenciales", sql, new { email, contraseña });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { email, contraseña });
        stopwatch.Stop();
        
        LogExecutionTime("VerificarCredenciales", stopwatch.Elapsed);
        return result;
    }

    public bool CambiarContraseña(uint idUsuario, string nuevaContraseña)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Usuario SET Contrasenia = SHA2(@nuevaContraseña, 256) WHERE idUsuario = @idUsuario";
        
        LogQuery("CambiarContraseña", sql, new { idUsuario, nuevaContraseña });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new { idUsuario, nuevaContraseña });
        stopwatch.Stop();
        
        LogExecutionTime("CambiarContraseña", stopwatch.Elapsed);
        return result > 0;
    }
    
    public async Task<Usuario?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.idNacionalidad = n.idNacionalidad
            WHERE u.idUsuario = @id";

        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Usuario, Nacionalidad, Usuario>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (usuario, nacionalidad) =>
            {
                usuario.Nacionalidad = nacionalidad;
                return usuario;
            }, splitOn: "idNacionalidad");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Usuario>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerUsuarios");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Usuario>(
            new CommandDefinition("ObtenerUsuarios", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Usuario>> BuscarAsync(Expression<Func<Usuario, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Usuario entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unNombreUsuario", entidad.NombreUsuario);
        parameters.Add("unEmail", entidad.Email);
        parameters.Add("unaContrasenia", entidad.Contrasenia);
        parameters.Add("unidNacionalidad", entidad.Nacionalidad.idNacionalidad);
        parameters.Add("unidUsuario", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaUsuario", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaUsuario", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.idUsuario = parameters.Get<uint>("unidUsuario");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Usuario entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Usuario 
                   SET NombreUsuario = @NombreUsuario,
                       Email = @Email,
                       Contrasenia = @Contrasenia,
                       idNacionalidad = @idNacionalidad
                   WHERE idUsuario = @idUsuario";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                entidad.NombreUsuario,
                entidad.Email,
                entidad.Contrasenia,
                idNacionalidad = entidad.Nacionalidad.idNacionalidad,
                entidad.idUsuario
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Usuario WHERE idUsuario = @id";
        
        LogQuery("EliminarAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Usuario";
        
        LogQuery("ContarAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ContarAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Usuario WHERE idUsuario = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.idNacionalidad = n.idNacionalidad
            WHERE u.Email = @email";

        LogQuery("ObtenerPorEmailAsync", sql, new { email });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Usuario, Nacionalidad, Usuario>(
            new CommandDefinition(sql, new { email }, cancellationToken: cancellationToken),
            (usuario, nacionalidad) =>
            {
                usuario.Nacionalidad = nacionalidad;
                return usuario;
            }, splitOn: "idNacionalidad");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorEmailAsync", stopwatch.Elapsed);
        
        return result.FirstOrDefault();
    }

    public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.idNacionalidad = n.idNacionalidad
            WHERE u.NombreUsuario = @nombreUsuario";

        LogQuery("ObtenerPorNombreUsuarioAsync", sql, new { nombreUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Usuario, Nacionalidad, Usuario>(
            new CommandDefinition(sql, new { nombreUsuario }, cancellationToken: cancellationToken),
            (usuario, nacionalidad) =>
            {
                usuario.Nacionalidad = nacionalidad;
                return usuario;
            }, splitOn: "idNacionalidad");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorNombreUsuarioAsync", stopwatch.Elapsed);
        
        return result.FirstOrDefault();
    }

    public async Task<Usuario?> ObtenerConPlaylistsAsync(uint idUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*, p.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.idNacionalidad = n.idNacionalidad
            LEFT JOIN Playlist p ON u.idUsuario = p.idUsuario
            WHERE u.idUsuario = @idUsuario";

        LogQuery("ObtenerConPlaylistsAsync", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var usuarios = new Dictionary<uint, Usuario>();
        var result = await connection.QueryAsync<Usuario, Nacionalidad, PlayList, Usuario>(
            new CommandDefinition(sql, new { idUsuario }, cancellationToken: cancellationToken),
            (usuario, nacionalidad, playlist) =>
            {
                if (!usuarios.TryGetValue(usuario.idUsuario, out var usuarioEntry))
                {
                    usuarioEntry = usuario;
                    usuarioEntry.Nacionalidad = nacionalidad;
                    usuarioEntry.Playlists = new List<PlayList>();
                    usuarios.Add(usuarioEntry.idUsuario, usuarioEntry);
                }
                
                if (playlist != null)
                {
                    usuarioEntry.Playlists.Add(playlist);
                }
                
                return usuarioEntry;
            }, splitOn: "idNacionalidad,idPlaylist");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConPlaylistsAsync", stopwatch.Elapsed);
        
        return usuarios.Values.FirstOrDefault();
    }

    public async Task<Usuario?> ObtenerConSuscripcionesAsync(uint idUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*, r.*, ts.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.idNacionalidad = n.idNacionalidad
            LEFT JOIN Suscripcion r ON u.idUsuario = r.idUsuario
            LEFT JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE u.idUsuario = @idUsuario";
            
        LogQuery("ObtenerConSuscripcionesAsync", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var usuarios = new Dictionary<uint, Usuario>();
        var result = await connection.QueryAsync<Usuario, Nacionalidad, Registro, TipoSuscripcion, Usuario>(
            new CommandDefinition(sql, new { idUsuario }, cancellationToken: cancellationToken),
            (usuario, nacionalidad, registro, tipoSuscripcion) =>
            {
                if (!usuarios.TryGetValue(usuario.idUsuario, out var usuarioEntry))
                {
                    usuarioEntry = usuario;
                    usuarioEntry.Nacionalidad = nacionalidad;
                    usuarioEntry.Suscripciones = new List<Registro>();
                    usuarios.Add(usuarioEntry.idUsuario, usuarioEntry);
                }
                
                if (registro != null)
                {
                    registro.TipoSuscripcion = tipoSuscripcion;
                    usuarioEntry.Suscripciones.Add(registro);
                }
                
                return usuarioEntry;
            }, splitOn: "idNacionalidad,idSuscripcion,idTipoSuscripcion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConSuscripcionesAsync", stopwatch.Elapsed);
        
        return usuarios.Values.FirstOrDefault();
    }

    public async Task<bool> VerificarCredencialesAsync(string email, string contraseña, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Usuario WHERE Email = @email AND Contrasenia = SHA2(@contraseña, 256)";
        
        LogQuery("VerificarCredencialesAsync", sql, new { email, contraseña });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { email, contraseña }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("VerificarCredencialesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> CambiarContraseñaAsync(uint idUsuario, string nuevaContraseña, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Usuario SET Contrasenia = SHA2(@nuevaContraseña, 256) WHERE idUsuario = @idUsuario";
        
        LogQuery("CambiarContraseñaAsync", sql, new { idUsuario, nuevaContraseña });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { idUsuario, nuevaContraseña }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("CambiarContraseñaAsync", stopwatch.Elapsed);
        return result > 0;
    }
}