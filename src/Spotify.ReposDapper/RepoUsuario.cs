namespace Spotify.ReposDapper;

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
            JOIN Nacionalidad n ON u.IdNacionalidad = n.IdNacionalidad
            WHERE u.IdUsuario = @id";
        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Usuario, Nacionalidad, Usuario>(sql,
            (usuario, nacionalidad) =>
            {
                usuario.Nacionalidad = nacionalidad;
                return usuario;
            }, new { id }, splitOn: "IdNacionalidad").FirstOrDefault();
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
        parameters.Add("unidNacionalidad", entidad.Nacionalidad.IdNacionalidad);
        parameters.Add("unidUsuario", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaUsuario", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaUsuario", parameters, commandType: CommandType.StoredProcedure);
        entidad.IdUsuario = parameters.Get<uint>("unidUsuario");
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
                       IdNacionalidad = @IdNacionalidad
                   WHERE IdUsuario = @IdUsuario";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            entidad.NombreUsuario,
            entidad.Email,
            entidad.Contrasenia,
            IdNacionalidad = entidad.Nacionalidad.IdNacionalidad,
            entidad.IdUsuario
        });
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Usuario WHERE IdUsuario = @id";
        
        LogQuery("Eliminar", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Usuario entidad)
    {
        Eliminar(entidad.IdUsuario);
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
        var sql = "SELECT COUNT(1) FROM Usuario WHERE IdUsuario = @id";
        
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
            JOIN Nacionalidad n ON u.IdNacionalidad = n.IdNacionalidad
            WHERE u.Email = @email";

        LogQuery("ObtenerPorEmail", sql, new { email });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Usuario, Nacionalidad, Usuario>(sql,
            (usuario, nacionalidad) =>
            {
                usuario.Nacionalidad = nacionalidad;
                return usuario;
            }, new { email }, splitOn: "IdNacionalidad").FirstOrDefault();
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
            JOIN Nacionalidad n ON u.IdNacionalidad = n.IdNacionalidad
            WHERE u.NombreUsuario = @nombreUsuario";

        LogQuery("ObtenerPorNombreUsuario", sql, new { nombreUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Usuario, Nacionalidad, Usuario>(sql,
            (usuario, nacionalidad) =>
            {
                usuario.Nacionalidad = nacionalidad;
                return usuario;
            }, new { nombreUsuario }, splitOn: "IdNacionalidad").FirstOrDefault();
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorNombreUsuario", stopwatch.Elapsed);
        return result;
    }

    public Usuario? ObtenerConPlaylists(uint IdUsuario)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*, p.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.IdNacionalidad = n.IdNacionalidad
            LEFT JOIN Playlist p ON u.IdUsuario = p.IdUsuario
            WHERE u.IdUsuario = @IdUsuario";

        LogQuery("ObtenerConPlaylists", sql, new { IdUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var usuarios = new Dictionary<uint, Usuario>();
        var result = connection.Query<Usuario, Nacionalidad, Playlist, Usuario>(sql,
            (usuario, nacionalidad, playlist) =>
            {
                if (!usuarios.TryGetValue(usuario.IdUsuario, out var usuarioEntry))
                {
                    usuarioEntry = usuario;
                    usuarioEntry.Nacionalidad = nacionalidad;
                    usuarioEntry.Playlists = new List<Playlist>();
                    usuarios.Add(usuarioEntry.IdUsuario, usuarioEntry);
                }
                
                if (playlist != null)
                {
                    usuarioEntry.Playlists.Add(playlist);
                }
                
                return usuarioEntry;
            }, new { IdUsuario }, splitOn: "IdNacionalidad,idPlaylist");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConPlaylists", stopwatch.Elapsed);
        
        return usuarios.Values.FirstOrDefault();
    }

    public Usuario? ObtenerConSuscripciones(uint IdUsuario)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*, r.*, ts.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.IdNacionalidad = n.IdNacionalidad
            LEFT JOIN Suscripcion r ON u.IdUsuario = r.IdUsuario
            LEFT JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE u.IdUsuario = @IdUsuario";

        LogQuery("ObtenerConSuscripciones", sql, new { IdUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var usuarios = new Dictionary<uint, Usuario>();
        var result = connection.Query<Usuario, Nacionalidad, Suscripcion, TipoSuscripcion, Usuario>(sql,
            (usuario, nacionalidad, registro, tipoSuscripcion) =>
            {
                if (!usuarios.TryGetValue(usuario.IdUsuario, out var usuarioEntry))
                {
                    usuarioEntry = usuario;
                    usuarioEntry.Nacionalidad = nacionalidad;
                    usuarioEntry.Suscripciones = new List<Suscripcion>();
                    usuarios.Add(usuarioEntry.IdUsuario, usuarioEntry);
                }
                
                if (registro != null)
                {
                    registro.TipoSuscripcion = tipoSuscripcion;
                    usuarioEntry.Suscripciones.Add(registro);
                }
                
                return usuarioEntry;
            }, new { IdUsuario }, splitOn: "IdNacionalidad,idSuscripcion,idTipoSuscripcion");
        
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

    public bool CambiarContraseña(uint IdUsuario, string nuevaContraseña)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Usuario SET Contrasenia = SHA2(@nuevaContraseña, 256) WHERE IdUsuario = @IdUsuario";
        
        LogQuery("CambiarContraseña", sql, new { IdUsuario, nuevaContraseña });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new { IdUsuario, nuevaContraseña });
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
            JOIN Nacionalidad n ON u.IdNacionalidad = n.IdNacionalidad
            WHERE u.IdUsuario = @id";

        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Usuario, Nacionalidad, Usuario>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (usuario, nacionalidad) =>
            {
                usuario.Nacionalidad = nacionalidad;
                return usuario;
            }, splitOn: "IdNacionalidad");
        
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
        parameters.Add("unidNacionalidad", entidad.Nacionalidad.IdNacionalidad);
        parameters.Add("unidUsuario", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaUsuario", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaUsuario", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.IdUsuario = parameters.Get<uint>("unidUsuario");
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
                       IdNacionalidad = @IdNacionalidad
                   WHERE IdUsuario = @IdUsuario";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                entidad.NombreUsuario,
                entidad.Email,
                entidad.Contrasenia,
                IdNacionalidad = entidad.Nacionalidad.IdNacionalidad,
                entidad.IdUsuario
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Usuario WHERE IdUsuario = @id";
        
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
        var sql = "SELECT COUNT(1) FROM Usuario WHERE IdUsuario = @id";
        
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
            JOIN Nacionalidad n ON u.IdNacionalidad = n.IdNacionalidad
            WHERE u.Email = @email";

        LogQuery("ObtenerPorEmailAsync", sql, new { email });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Usuario, Nacionalidad, Usuario>(
            new CommandDefinition(sql, new { email }, cancellationToken: cancellationToken),
            (usuario, nacionalidad) =>
            {
                usuario.Nacionalidad = nacionalidad;
                return usuario;
            }, splitOn: "IdNacionalidad");
        
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
            JOIN Nacionalidad n ON u.IdNacionalidad = n.IdNacionalidad
            WHERE u.NombreUsuario = @nombreUsuario";

        LogQuery("ObtenerPorNombreUsuarioAsync", sql, new { nombreUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Usuario, Nacionalidad, Usuario>(
            new CommandDefinition(sql, new { nombreUsuario }, cancellationToken: cancellationToken),
            (usuario, nacionalidad) =>
            {
                usuario.Nacionalidad = nacionalidad;
                return usuario;
            }, splitOn: "IdNacionalidad");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorNombreUsuarioAsync", stopwatch.Elapsed);
        
        return result.FirstOrDefault();
    }

    public async Task<Usuario?> ObtenerConPlaylistsAsync(uint IdUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*, p.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.IdNacionalidad = n.IdNacionalidad
            LEFT JOIN Playlist p ON u.IdUsuario = p.IdUsuario
            WHERE u.IdUsuario = @IdUsuario";

        LogQuery("ObtenerConPlaylistsAsync", sql, new { IdUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var usuarios = new Dictionary<uint, Usuario>();
        var result = await connection.QueryAsync<Usuario, Nacionalidad, Playlist, Usuario>(
            new CommandDefinition(sql, new { IdUsuario }, cancellationToken: cancellationToken),
            (usuario, nacionalidad, playlist) =>
            {
                if (!usuarios.TryGetValue(usuario.IdUsuario, out var usuarioEntry))
                {
                    usuarioEntry = usuario;
                    usuarioEntry.Nacionalidad = nacionalidad;
                    usuarioEntry.Playlists = new List<Playlist>();
                    usuarios.Add(usuarioEntry.IdUsuario, usuarioEntry);
                }
                
                if (playlist != null)
                {
                    usuarioEntry.Playlists.Add(playlist);
                }
                
                return usuarioEntry;
            }, splitOn: "IdNacionalidad,idPlaylist");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConPlaylistsAsync", stopwatch.Elapsed);
        
        return usuarios.Values.FirstOrDefault();
    }

    public async Task<Usuario?> ObtenerConSuscripcionesAsync(uint IdUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT u.*, n.*, r.*, ts.*
            FROM Usuario u
            JOIN Nacionalidad n ON u.IdNacionalidad = n.IdNacionalidad
            LEFT JOIN Suscripcion r ON u.IdUsuario = r.IdUsuario
            LEFT JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE u.IdUsuario = @IdUsuario";
            
        LogQuery("ObtenerConSuscripcionesAsync", sql, new { IdUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var usuarios = new Dictionary<uint, Usuario>();
        var result = await connection.QueryAsync<Usuario, Nacionalidad, Suscripcion, TipoSuscripcion, Usuario>(
            new CommandDefinition(sql, new { IdUsuario }, cancellationToken: cancellationToken),
            (usuario, nacionalidad, registro, tipoSuscripcion) =>
            {
                if (!usuarios.TryGetValue(usuario.IdUsuario, out var usuarioEntry))
                {
                    usuarioEntry = usuario;
                    usuarioEntry.Nacionalidad = nacionalidad;
                    usuarioEntry.Suscripciones = new List<Suscripcion>();
                    usuarios.Add(usuarioEntry.IdUsuario, usuarioEntry);
                }
                
                if (registro != null)
                {
                    registro.TipoSuscripcion = tipoSuscripcion;
                    usuarioEntry.Suscripciones.Add(registro);
                }
                
                return usuarioEntry;
            }, splitOn: "IdNacionalidad,idSuscripcion,idTipoSuscripcion");
        
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

    public async Task<bool> CambiarContraseñaAsync(uint IdUsuario, string nuevaContraseña, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Usuario SET Contrasenia = SHA2(@nuevaContraseña, 256) WHERE IdUsuario = @IdUsuario";
        
        LogQuery("CambiarContraseñaAsync", sql, new { IdUsuario, nuevaContraseña });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { IdUsuario, nuevaContraseña }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("CambiarContraseñaAsync", stopwatch.Elapsed);
        return result > 0;
    }

    public Task ObtenerNuevosHoyAsync()
    {
        throw new NotImplementedException();
    }

    public Task ObtenerTotalAsync()
    {
        throw new NotImplementedException();
    }

    public Task<object?> ObtenerReporteUsuariosAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        throw new NotImplementedException();
    }
}