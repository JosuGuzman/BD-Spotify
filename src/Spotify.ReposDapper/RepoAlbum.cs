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
            JOIN Artista art ON a.IdArtista = art.IdArtista
            WHERE a.IdAlbum = @id";

        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Album, Artista, Album>(sql,
            (album, artista) =>
            {
                album.Artista = artista;
                return album;
            }, new { id }, splitOn: "IdArtista").FirstOrDefault();
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
        parameters.Add("unidArtista", entidad.Artista.IdArtista);
        parameters.Add("unPortada", entidad.Portada);
        parameters.Add("unidAlbum", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaAlbum", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaAlbum", parameters, commandType: CommandType.StoredProcedure);
        entidad.IdAlbum = parameters.Get<uint>("unidAlbum");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Album entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Album 
                   SET Titulo = @Titulo,
                       fechaLanzamiento = @FechaLanzamiento,
                       IdArtista = @IdArtista,
                       Portada = @Portada
                   WHERE IdAlbum = @IdAlbum";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            entidad.Titulo,
            entidad.FechaLanzamiento,
            IdArtista = entidad.Artista.IdArtista,
            entidad.Portada,
            entidad.IdAlbum
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
        Eliminar(entidad.IdAlbum);
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
        var sql = "SELECT COUNT(1) FROM Album WHERE IdAlbum = @id";
        
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

    public IEnumerable<Album> ObtenerPaginado(int pagina, int tamañoPagina, string ordenarPor = "IdAlbum")
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
            JOIN Artista art ON a.IdArtista = art.IdArtista
            LEFT JOIN Cancion c ON a.IdAlbum = c.IdAlbum";

        LogQuery("ObtenerConRelaciones", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var albumes = new Dictionary<uint, Album>();
        var result = connection.Query<Album, Artista, Cancion, Album>(sql,
            (album, artista, cancion) =>
            {
                if (!albumes.TryGetValue(album.IdAlbum, out var albumEntry))
                {
                    albumEntry = album;
                    albumEntry.Artista = artista;
                    albumEntry.Canciones = new List<Cancion>();
                    albumes.Add(albumEntry.IdAlbum, albumEntry);
                }
                
                if (cancion != null)
                {
                    albumEntry.Canciones.Add(cancion);
                }
                
                return albumEntry;
            }, splitOn: "IdArtista,idCancion");
        
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
                   JOIN Artista art ON a.IdArtista = art.IdArtista
                   ORDER BY a.fechaLanzamiento DESC
                   LIMIT @limite";

        LogQuery("ObtenerAlbumesRecientes", sql, new { limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Album, Artista, Album>(sql,
            (album, artista) =>
            {
                album.Artista = artista;
                return album;
            }, new { limite }, splitOn: "IdArtista");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerAlbumesRecientes", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Album> ObtenerPorArtista(uint IdArtista)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT a.*, art.*
            FROM Album a
            JOIN Artista art ON a.IdArtista = art.IdArtista
            WHERE a.IdArtista = @IdArtista";

        LogQuery("ObtenerPorArtista", sql, new { IdArtista });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Album, Artista, Album>(sql,
            (album, artista) =>
            {
                album.Artista = artista;
                return album;
            }, new { IdArtista }, splitOn: "IdArtista");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorArtista", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Album> ObtenerConCanciones()
    {
        return ObtenerConRelaciones();
    }

    public Album? ObtenerConArtistaYCanciones(uint IdAlbum)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT a.*, art.*, c.*, g.*, artCancion.*
            FROM Album a
            JOIN Artista art ON a.IdArtista = art.IdArtista
            LEFT JOIN Cancion c ON a.IdAlbum = c.IdAlbum
            LEFT JOIN Genero g ON c.idGenero = g.idGenero
            LEFT JOIN Artista artCancion ON c.IdArtista = artCancion.IdArtista
            WHERE a.IdAlbum = @IdAlbum";

        LogQuery("ObtenerConArtistaYCanciones", sql, new { IdAlbum });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var albumes = new Dictionary<uint, Album>();
        var result = connection.Query<Album, Artista, Cancion, Genero, Artista, Album>(sql,
            (album, artista, cancion, genero, artistaCancion) =>
            {
                if (!albumes.TryGetValue(album.IdAlbum, out var albumEntry))
                {
                    albumEntry = album;
                    albumEntry.Artista = artista;
                    albumEntry.Canciones = new List<Cancion>();
                    albumes.Add(albumEntry.IdAlbum, albumEntry);
                }
                
                if (cancion != null)
                {
                    cancion.Genero = genero;
                    cancion.Artista = artistaCancion;
                    albumEntry.Canciones.Add(cancion);
                }
                
                return albumEntry;
            }, new { IdAlbum }, splitOn: "IdArtista,idCancion,idGenero,IdArtista");
        
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
            JOIN Artista art ON a.IdArtista = art.IdArtista
            WHERE a.IdAlbum = @id";
            
        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Album, Artista, Album>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (album, artista) =>
            {
                album.Artista = artista;
                return album;
            }, splitOn: "IdArtista");
        
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
        parameters.Add("unidArtista", entidad.Artista.IdArtista);
        parameters.Add("unPortada", entidad.Portada);
        parameters.Add("unidAlbum", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaAlbum", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaAlbum", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.IdAlbum = parameters.Get<uint>("unidAlbum");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Album entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Album 
                   SET Titulo = @Titulo,
                       fechaLanzamiento = @FechaLanzamiento,
                       IdArtista = @IdArtista,
                       Portada = @Portada
                   WHERE IdAlbum = @IdAlbum";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                entidad.Titulo,
                entidad.FechaLanzamiento,
                IdArtista = entidad.Artista.IdArtista,
                entidad.Portada,
                entidad.IdAlbum
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
        var sql = "SELECT COUNT(1) FROM Album WHERE IdAlbum = @id";
        
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

    public async Task<IEnumerable<Album>> ObtenerPaginadoAsync(int pagina, int tamañoPagina, string ordenarPor = "IdAlbum", CancellationToken cancellationToken = default)
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
            JOIN Artista art ON a.IdArtista = art.IdArtista
            LEFT JOIN Cancion c ON a.IdAlbum = c.IdAlbum";

        LogQuery("ObtenerConRelacionesAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var albumes = new Dictionary<uint, Album>();
        var result = await connection.QueryAsync<Album, Artista, Cancion, Album>(
            new CommandDefinition(sql, cancellationToken: CancellationToken.None),
            (album, artista, cancion) =>
            {
                if (!albumes.TryGetValue(album.IdAlbum, out var albumEntry))
                {
                    albumEntry = album;
                    albumEntry.Artista = artista;
                    albumEntry.Canciones = new List<Cancion>();
                    albumes.Add(albumEntry.IdAlbum, albumEntry);
                }
                
                if (cancion != null)
                {
                    albumEntry.Canciones.Add(cancion);
                }
                
                return albumEntry;
            }, splitOn: "IdArtista,idCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelacionesAsync", stopwatch.Elapsed);
        
        return albumes.Values;
    }

    public async Task<IEnumerable<Album>> ObtenerAlbumesRecientesAsync(int limite = 10, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT a.*, art.*
                   FROM Album a
                   JOIN Artista art ON a.IdArtista = art.IdArtista
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
            }, splitOn: "IdArtista");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerAlbumesRecientesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Album>> ObtenerPorArtistaAsync(uint IdArtista, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT a.*, art.*
            FROM Album a
            JOIN Artista art ON a.IdArtista = art.IdArtista
            WHERE a.IdArtista = @IdArtista";

        LogQuery("ObtenerPorArtistaAsync", sql, new { IdArtista });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Album, Artista, Album>(
            new CommandDefinition(sql, new { IdArtista }, cancellationToken: cancellationToken),
            (album, artista) =>
            {
                album.Artista = artista;
                return album;
            }, splitOn: "IdArtista");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorArtistaAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Album>> ObtenerConCancionesAsync(CancellationToken cancellationToken = default)
    {
        return await ObtenerConRelacionesAsync();
    }

    public async Task<Album?> ObtenerConArtistaYCancionesAsync(uint IdAlbum, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT a.*, art.*, c.*, g.*, artCancion.*
            FROM Album a
            JOIN Artista art ON a.IdArtista = art.IdArtista
            LEFT JOIN Cancion c ON a.IdAlbum = c.IdAlbum
            LEFT JOIN Genero g ON c.idGenero = g.idGenero
            LEFT JOIN Artista artCancion ON c.IdArtista = artCancion.IdArtista
            WHERE a.IdAlbum = @IdAlbum";

        LogQuery("ObtenerConArtistaYCancionesAsync", sql, new { IdAlbum });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var albumes = new Dictionary<uint, Album>();
        var result = await connection.QueryAsync<Album, Artista, Cancion, Genero, Artista, Album>(
            new CommandDefinition(sql, new { IdAlbum }, cancellationToken: cancellationToken),
            (album, artista, cancion, genero, artistaCancion) =>
            {
                if (!albumes.TryGetValue(album.IdAlbum, out var albumEntry))
                {
                    albumEntry = album;
                    albumEntry.Artista = artista;
                    albumEntry.Canciones = new List<Cancion>();
                    albumes.Add(albumEntry.IdAlbum, albumEntry);
                }
                
                if (cancion != null)
                {
                    cancion.Genero = genero;
                    cancion.Artista = artistaCancion;
                    albumEntry.Canciones.Add(cancion);
                }
                
                return albumEntry;
            }, splitOn: "IdArtista,idCancion,idGenero,IdArtista");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConArtistaYCancionesAsync", stopwatch.Elapsed);
        
        return albumes.Values.FirstOrDefault();
    }
}