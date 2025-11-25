using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Spotify.Core.Persistencia;
using Spotify.Core.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Spotify.Core;
using Spotify.Core.Persistencia.Contracts;
using Spotify.Core.Models.Analiticas;

namespace Spotify.Core;

public class Album
{
    [Key]
    public uint idAlbum { get; set; }

    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(45, ErrorMessage = "El título no puede exceder 45 caracteres")]
    public required string Titulo { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaLanzamiento { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "El artista es requerido")]
    public required Artista Artista { get; set; }

    public string Portada { get; set; } = "default_album.png";

    // Propiedades de navegación
    [JsonIgnore]
    public virtual ICollection<Cancion>? Canciones { get; set; }

    // Propiedades calculadas
    public int TotalCanciones => Canciones?.Count ?? 0;
    
    public TimeSpan DuracionTotal => Canciones?.Aggregate(TimeSpan.Zero, 
        (total, cancion) => total + cancion.Duracion) ?? TimeSpan.Zero;
        
    public bool EsReciente => FechaLanzamiento >= DateTime.Now.AddMonths(-6);
}

public class Artista
{
    [Key]
    public uint idArtista { get; set; }

    [StringLength(35, ErrorMessage = "El nombre artístico no puede exceder 35 caracteres")]
    public string? NombreArtistico { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(45, ErrorMessage = "El nombre no puede exceder 45 caracteres")]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = "El apellido es requerido")]
    [StringLength(45, ErrorMessage = "El apellido no puede exceder 45 caracteres")]
    public required string Apellido { get; set; }

    // Propiedades de navegación
    [JsonIgnore]
    public virtual ICollection<Album>? Albumes { get; set; }
        
    [JsonIgnore]
    public virtual ICollection<Cancion>? Canciones { get; set; }

    // Propiedad calculada
    public string NombreCompleto => $"{Nombre} {Apellido}";
        
    public string DisplayName => NombreArtistico ?? NombreCompleto;
}

public class Cancion
{
    [Key]
    public uint idCancion { get; set; }

    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(45, ErrorMessage = "El título no puede exceder 45 caracteres")]
    public required string Titulo { get; set; }

    public TimeSpan Duracion { get; set; }

    [Required(ErrorMessage = "El álbum es requerido")]
    public required Album Album { get; set; }

    [Required(ErrorMessage = "El género es requerido")]
    public required Genero Genero { get; set; }

    [Required(ErrorMessage = "El artista es requerido")]
    public required Artista Artista { get; set; }

    public string ArchivoMP3 { get; set; } = string.Empty;

    // Propiedades de navegación
    [JsonIgnore]
    public virtual ICollection<PlayList>? Playlists { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Reproduccion>? Reproducciones { get; set; }

    // Propiedades calculadas
    public string DuracionFormateada => $"{(int)Duracion.TotalMinutes}:{Duracion.Seconds:D2}";
    
    public int TotalReproducciones => Reproducciones?.Count ?? 0;
    
    public bool TieneArchivo => !string.IsNullOrEmpty(ArchivoMP3) && ArchivoMP3 != "default.mp3";
}

public class CancionPlaylist
{
    [Required(ErrorMessage = "La playlist es requerida")]
    public required PlayList Playlist { get; set; }

    [Required(ErrorMessage = "La canción es requerida")]
    public required Cancion Cancion { get; set; }

    // Propiedades adicionales para la relación
    public DateTime FechaAgregada { get; set; } = DateTime.Now;
    
    public uint Orden { get; set; }

    // Propiedades calculadas
    [JsonIgnore]
    public string Info => $"{Cancion.Titulo} en {Playlist.Nombre}";
    
    public bool EsReciente => FechaAgregada >= DateTime.Now.AddDays(-7);
}

public class Genero
{
    [Key]
    public byte idGenero { get; set; }

    [Required(ErrorMessage = "El género es requerido")]
    [StringLength(45, ErrorMessage = "El género no puede exceder 45 caracteres")]
    public required string Nombre { get; set; } // Cambiado de 'genero' a 'Nombre'

    public string Descripcion { get; set; } = string.Empty;

    // Propiedades de navegación
    [JsonIgnore]
    public virtual ICollection<Cancion>? Canciones { get; set; }

    // Propiedad calculada
    public int TotalCanciones => Canciones?.Count ?? 0;
}

public class Nacionalidad
{
    [Key]
    public uint idNacionalidad { get; set; }

    [Required(ErrorMessage = "El país es requerido")]
    [StringLength(45, ErrorMessage = "El país no puede exceder 45 caracteres")]
    public required string Pais { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Usuario>? Usuarios { get; set; }
}

public class PlayList
{
    [Key]
    public uint idPlaylist { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(20, ErrorMessage = "El nombre no puede exceder 20 caracteres")]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = "El usuario es requerido")]
    public required Usuario Usuario { get; set; }

    public virtual ICollection<Cancion> Canciones { get; set; } = new List<Cancion>();

    // Propiedades calculadas
    public int TotalCanciones => Canciones.Count;
    
    public TimeSpan DuracionTotal => Canciones.Aggregate(TimeSpan.Zero, 
        (total, cancion) => total + cancion.Duracion);
        
    public DateTime? FechaCreacion { get; set; } = DateTime.Now;
    
    public bool EsFavoritos => Nombre.Equals("Tus Megusta", StringComparison.OrdinalIgnoreCase);
}

public class Registro
{
    [Key]
    public uint IdSuscripcion { get; set; }

    [Required(ErrorMessage = "El usuario es requerido")]
    public required Usuario Usuario { get; set; }

    [Required(ErrorMessage = "El tipo de suscripción es requerido")]
    public required TipoSuscripcion TipoSuscripcion { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; } = DateTime.Now;

    // Propiedades adicionales
    public DateTime? FechaRenovacion { get; set; }
    
    public bool AutoRenovacion { get; set; } = true;
    
    public string? MetodoPago { get; set; }

    // Propiedades calculadas
    public DateTime FechaExpiracion => FechaInicio.AddMonths((int)TipoSuscripcion.Duracion);
    
    public bool EstaActiva => FechaExpiracion >= DateTime.Now;
    
    public bool ExpiraProximamente => FechaExpiracion <= DateTime.Now.AddDays(7) && FechaExpiracion > DateTime.Now;
    
    public int DiasRestantes => (FechaExpiracion - DateTime.Now).Days;
    
    public string Estado => EstaActiva ? 
        (ExpiraProximamente ? "Por expirar" : "Activa") : "Expirada";
}

public class Reproduccion
{
    [Key]
    public uint IdHistorial { get; set; }

    [Required(ErrorMessage = "El usuario es requerido")]
    public required Usuario Usuario { get; set; }

    [Required(ErrorMessage = "La canción es requerida")]
    public required Cancion Cancion { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime FechaReproduccion { get; set; } = DateTime.Now;

    // Propiedades adicionales para analytics
    public TimeSpan ProgresoReproduccion { get; set; }
    
    public bool ReproduccionCompleta { get; set; }
    
    public string? Dispositivo { get; set; }

    // Propiedades calculadas
    public double PorcentajeEscuchado => 
        ReproduccionCompleta ? 100 : (ProgresoReproduccion.TotalSeconds / Cancion.Duracion.TotalSeconds) * 100;
    
    public bool EsHoy => FechaReproduccion.Date == DateTime.Today;
    
    public string DispositivoFormateado => Dispositivo ?? "Web Player";
}

public class TipoSuscripcion
{
    [Key]
    public uint IdTipoSuscripcion { get; set; }

    [Required(ErrorMessage = "La duración es requerida")]
    [Range(1, 12, ErrorMessage = "La duración debe estar entre 1 y 12 meses")]
    public byte Duracion { get; set; }

    [Required(ErrorMessage = "El costo es requerido")]
    [Range(1, 100, ErrorMessage = "El costo debe estar entre 1 y 100")]
    public byte Costo { get; set; }

    [Required(ErrorMessage = "El tipo es requerido")]
    [StringLength(45, ErrorMessage = "El tipo no puede exceder 45 caracteres")]
    public required string Tipo { get; set; }

    // Propiedades de navegación
    [JsonIgnore]
    public virtual ICollection<Registro>? Registros { get; set; }

    // Propiedades calculadas
    public decimal CostoMensual => Costo / (decimal)Duracion;
    
    public string DescripcionCompleta => $"{Tipo} - ${Costo} por {Duracion} mes(es)";
    
    public bool EsPopular => Costo <= 10; // Suscripciones económicas son populares
}

public class Usuario
{
    [Key]
    public uint idUsuario { get; set; }
    
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(45, ErrorMessage = "El nombre de usuario no puede exceder 45 caracteres")]
    public required string NombreUsuario { get; set; }

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(45, ErrorMessage = "El email no puede exceder 45 caracteres")]
    public required string Email { get; set; } // Cambiado de Gmail a Email para coincidir con BD
    
    [Required(ErrorMessage = "La contraseña es requerida")]
    [JsonIgnore] // No exponer la contraseña en respuestas JSON
    public required string Contrasenia { get; set; }
    
    [Required(ErrorMessage = "La nacionalidad es requerida")]
    public required Nacionalidad Nacionalidad { get; set; }
    
    // Propiedades de navegación
    
    [JsonIgnore]
    public virtual ICollection<PlayList>? Playlists { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Reproduccion>? Reproducciones { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Registro>? Suscripciones { get; set; }
    
    // Propiedades calculadas
    
    public bool EstaActivo => Suscripciones?.Any(s => 
        s.FechaInicio.AddMonths((int)s.TipoSuscripcion.Duracion) >= DateTime.Now) ?? false;
}

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

namespace Spotify.Core.Models.Analiticas;

public class EstadisticasGlobales
{
    public int TotalUsuarios { get; set; }
    public int TotalCanciones { get; set; }
    public int TotalArtistas { get; set; }
    public int TotalAlbumes { get; set; }
    public int TotalReproduccionesHoy { get; set; }
    public int TotalReproduccionesSemana { get; set; }
    public int TotalReproduccionesMes { get; set; }
    public IEnumerable<Cancion> CancionesMasPopulares { get; set; } = new List<Cancion>();
    public IEnumerable<Artista> ArtistasMasPopulares { get; set; } = new List<Artista>();
    public Dictionary<string, int> DistribucionGeneros { get; set; } = new();
}

public class EstadisticasUsuario
{
    public uint IdUsuario { get; set; }
    public int TotalReproducciones { get; set; }
    public TimeSpan TiempoTotalEscuchado { get; set; }
    public int TotalArtistasEscuchados { get; set; }
    public int TotalGenerosEscuchados { get; set; }
    public IEnumerable<Cancion> TopCanciones { get; set; } = new List<Cancion>();
    public IEnumerable<Artista> TopArtistas { get; set; } = new List<Artista>();
    public IEnumerable<Genero> TopGeneros { get; set; } = new List<Genero>();
    public Dictionary<DayOfWeek, int> ReproduccionesPorDia { get; set; } = new();
    public Dictionary<int, int> ReproduccionesPorHora { get; set; } = new();
}

public class TopArtista
{
    public Artista? Artista { get; set; }
    public int TotalReproducciones { get; set; }
    public int TotalSeguidores { get; set; }
}

public class TopCancion
{
    public Cancion? Cancion { get; set; }
    public int TotalReproducciones { get; set; }
    public int TotalLikes { get; set; }
}

namespace Spotify.Core.Models.Busqueda;

public class FiltroBusqueda
{
    public string? Termino { get; set; }
    public byte? IdGenero { get; set; }
    public uint? IdArtista { get; set; }
    public uint? IdAlbum { get; set; }
    public int? AnoMin { get; set; }
    public int? AnoMax { get; set; }
    public TimeSpan? DuracionMin { get; set; }
    public TimeSpan? DuracionMax { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamañoPagina { get; set; } = 20;
    public string OrdenarPor { get; set; } = "relevancia";
    public bool OrdenAscendente { get; set; } = false;
}

public class ResultadoBusqueda
{
    public IEnumerable<Cancion> Canciones { get; set; } = new List<Cancion>();
    public IEnumerable<Album> Albumes { get; set; } = new List<Album>();
    public IEnumerable<Artista> Artistas { get; set; } = new List<Artista>();
    public IEnumerable<PlayList> Playlists { get; set; } = new List<PlayList>();
    public IEnumerable<Genero> Generos { get; set; } = new List<Genero>();
    
    public int TotalResultados => Canciones.Count() + Albumes.Count() + Artistas.Count() + Playlists.Count() + Generos.Count();
}

namespace Spotify.Core.Services.Business;

public class ServicioAnaliticas : IServicioAnaliticas
{
    private readonly IRepoReproduccion _repoReproduccion;
    private readonly IRepoUsuario _repoUsuario;
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoArtista _repoArtista;
    private readonly IRepoAlbum _repoAlbum;
    private readonly ILogger<ServicioAnaliticas> _logger;
    private readonly ICacheService _cacheService;

    public ServicioAnaliticas(
        IRepoReproduccion repoReproduccion,
        IRepoUsuario repoUsuario,
        IRepoCancion repoCancion,
        IRepoArtista repoArtista,
        IRepoAlbum repoAlbum,
        ILogger<ServicioAnaliticas> logger,
        ICacheService cacheService)
    {
        _repoReproduccion = repoReproduccion;
        _repoUsuario = repoUsuario;
        _repoCancion = repoCancion;
        _repoArtista = repoArtista;
        _repoAlbum = repoAlbum;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<EstadisticasUsuario> ObtenerEstadisticasUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"estadisticas_usuario_{idUsuario}";
        
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogInformation("Calculando estadísticas para usuario: {UsuarioId}", idUsuario);

            var usuario = await _repoUsuario.ObtenerPorIdAsync(idUsuario, cancellationToken);
            if (usuario == null)
            {
                _logger.LogWarning("Usuario no encontrado: {UsuarioId}", idUsuario);
                throw new ArgumentException($"Usuario con ID {idUsuario} no encontrado");
            }

            var historial = await _repoReproduccion.ObtenerHistorialUsuarioAsync(idUsuario, 1000, cancellationToken);
            var reproduccionesList = historial.ToList();

            if (!reproduccionesList.Any())
            {
                _logger.LogDebug("Usuario {UsuarioId} no tiene reproducciones", idUsuario);
                return new EstadisticasUsuario { IdUsuario = idUsuario };
            }

            // Cálculos de estadísticas
            var totalReproducciones = reproduccionesList.Count;
            var tiempoTotal = TimeSpan.FromSeconds(reproduccionesList.Sum(r => r.Cancion.Duracion.TotalSeconds));
            var artistasUnicos = reproduccionesList.Select(r => r.Cancion.Artista.idArtista).Distinct().Count();
            var generosUnicos = reproduccionesList.Select(r => r.Cancion.Genero.idGenero).Distinct().Count();

            // Top canciones
            var topCanciones = reproduccionesList
                .GroupBy(r => r.Cancion)
                .Select(g => new { Cancion = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .Select(x => x.Cancion)
                .ToList();

            // Top artistas
            var topArtistas = reproduccionesList
                .GroupBy(r => r.Cancion.Artista)
                .Select(g => new { Artista = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Select(x => x.Artista)
                .ToList();

            // Top géneros
            var topGeneros = reproduccionesList
                .GroupBy(r => r.Cancion.Genero)
                .Select(g => new { Genero = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Select(x => x.Genero)
                .ToList();

            // Estadísticas por día de la semana
            var reproduccionesPorDia = reproduccionesList
                .GroupBy(r => r.FechaReproduccion.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.Count());

            // Estadísticas por hora del día
            var reproduccionesPorHora = reproduccionesList
                .GroupBy(r => r.FechaReproduccion.Hour)
                .ToDictionary(g => g.Key, g => g.Count());

            var estadisticas = new EstadisticasUsuario
            {
                IdUsuario = idUsuario,
                TotalReproducciones = totalReproducciones,
                TiempoTotalEscuchado = tiempoTotal,
                TotalArtistasEscuchados = artistasUnicos,
                TotalGenerosEscuchados = generosUnicos,
                TopCanciones = topCanciones,
                TopArtistas = topArtistas,
                TopGeneros = topGeneros,
                ReproduccionesPorDia = reproduccionesPorDia,
                ReproduccionesPorHora = reproduccionesPorHora
            };

            _logger.LogInformation("Estadísticas calculadas para usuario {UsuarioId}: {Reproducciones} reproducciones, {Tiempo} tiempo total", 
                idUsuario, totalReproducciones, tiempoTotal);

            return estadisticas;
        }, TimeSpan.FromMinutes(30));
    }

    public async Task<EstadisticasGlobales> ObtenerEstadisticasGlobalesAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "estadisticas_globales";
        
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogInformation("Calculando estadísticas globales del sistema");

            var usuarios = await _repoUsuario.ObtenerTodosAsync(cancellationToken);
            var canciones = await _repoCancion.ObtenerTodosAsync(cancellationToken);
            var artistas = await _repoArtista.ObtenerTodosAsync(cancellationToken);
            var albumes = await _repoAlbum.ObtenerTodosAsync(cancellationToken);

            var fechaHoy = DateTime.Today;
            var fechaSemanaPasada = fechaHoy.AddDays(-7);
            var fechaMesPasado = fechaHoy.AddDays(-30);

            var reproduccionesRecientes = await _repoReproduccion.ObtenerRecientesAsync(fechaMesPasado, cancellationToken);
            var reproduccionesList = reproduccionesRecientes.ToList();

            var totalReproduccionesHoy = reproduccionesList.Count(r => r.FechaReproduccion.Date == fechaHoy);
            var totalReproduccionesSemana = reproduccionesList.Count(r => r.FechaReproduccion >= fechaSemanaPasada);
            var totalReproduccionesMes = reproduccionesList.Count;

            // Canciones más populares
            var cancionesMasPopulares = reproduccionesList
                .GroupBy(r => r.Cancion)
                .Select(g => new { Cancion = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .Select(x => x.Cancion)
                .ToList();

            // Artistas más populares
            var artistasMasPopulares = reproduccionesList
                .GroupBy(r => r.Cancion.Artista)
                .Select(g => new { Artista = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .Select(x => x.Artista)
                .ToList();

            // Distribución de géneros
            var distribucionGeneros = reproduccionesList
                .GroupBy(r => r.Cancion.Genero.Nombre)
                .ToDictionary(g => g.Key, g => g.Count());

            var estadisticas = new EstadisticasGlobales
            {
                TotalUsuarios = usuarios.Count(),
                TotalCanciones = canciones.Count(),
                TotalArtistas = artistas.Count(),
                TotalAlbumes = albumes.Count(),
                TotalReproduccionesHoy = totalReproduccionesHoy,
                TotalReproduccionesSemana = totalReproduccionesSemana,
                TotalReproduccionesMes = totalReproduccionesMes,
                CancionesMasPopulares = cancionesMasPopulares,
                ArtistasMasPopulares = artistasMasPopulares,
                DistribucionGeneros = distribucionGeneros
            };

            _logger.LogInformation("Estadísticas globales calculadas: {Usuarios} usuarios, {Canciones} canciones, {ReproduccionesMes} reproducciones este mes",
                estadisticas.TotalUsuarios, estadisticas.TotalCanciones, estadisticas.TotalReproduccionesMes);

            return estadisticas;
        }, TimeSpan.FromMinutes(60));
    }

    public async Task<IEnumerable<TopArtista>> ObtenerTopArtistasAsync(DateTime desde, DateTime hasta, int limite = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Obteniendo top artistas desde {Desde} hasta {Hasta}", desde, hasta);

        var reproducciones = await _repoReproduccion.ObtenerRecientesAsync(desde, cancellationToken);
        var reproduccionesFiltradas = reproducciones.Where(r => r.FechaReproduccion >= desde && r.FechaReproduccion <= hasta);

        var topArtistas = reproduccionesFiltradas
            .GroupBy(r => r.Cancion.Artista)
            .Select(g => new TopArtista
            {
                Artista = g.Key,
                TotalReproducciones = g.Count(),
                TotalSeguidores = 0 // Esto requeriría una relación de seguidores
            })
            .OrderByDescending(x => x.TotalReproducciones)
            .Take(limite)
            .ToList();

        _logger.LogDebug("Top {Count} artistas calculados", topArtistas.Count);
        return topArtistas;
    }

    public async Task<IEnumerable<TopCancion>> ObtenerTopCancionesAsync(DateTime desde, DateTime hasta, int limite = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Obteniendo top canciones desde {Desde} hasta {Hasta}", desde, hasta);

        var reproducciones = await _repoReproduccion.ObtenerRecientesAsync(desde, cancellationToken);
        var reproduccionesFiltradas = reproducciones.Where(r => r.FechaReproduccion >= desde && r.FechaReproduccion <= hasta);

        var topCanciones = reproduccionesFiltradas
            .GroupBy(r => r.Cancion)
            .Select(g => new TopCancion
            {
                Cancion = g.Key,
                TotalReproducciones = g.Count(),
                TotalLikes = 0 // Esto requeriría una relación de likes
            })
            .OrderByDescending(x => x.TotalReproducciones)
            .Take(limite)
            .ToList();

        _logger.LogDebug("Top {Count} canciones calculadas", topCanciones.Count);
        return topCanciones;
    }
}

public class ServicioBusqueda : IServicioBusqueda
{
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoArtista _repoArtista;
    private readonly IRepoPlaylist _repoPlaylist;
    private readonly IRepoGenero _repoGenero;
    private readonly ILogger<ServicioBusqueda> _logger;
    private readonly ICacheService _cacheService;

    public ServicioBusqueda(
        IRepoCancion repoCancion,
        IRepoAlbum repoAlbum,
        IRepoArtista repoArtista,
        IRepoPlaylist repoPlaylist,
        IRepoGenero repoGenero,
        ILogger<ServicioBusqueda> logger,
        ICacheService cacheService)
    {
        _repoCancion = repoCancion;
        _repoAlbum = repoAlbum;
        _repoArtista = repoArtista;
        _repoPlaylist = repoPlaylist;
        _repoGenero = repoGenero;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<ResultadoBusqueda> BuscarGlobalAsync(string termino, int limite = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termino))
        {
            _logger.LogWarning("Intento de búsqueda con término vacío");
            return new ResultadoBusqueda();
        }

        var cacheKey = $"busqueda_global_{termino.ToLowerInvariant()}_{limite}";
        
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogInformation("Realizando búsqueda global: {Termino}", termino);
            
            try
            {
                // Ejecutar búsquedas en paralelo para mejor rendimiento
                var cancionesTask = _repoCancion.BuscarTextoAsync(termino, c => c.Titulo);
                var albumesTask = _repoAlbum.BuscarTextoAsync(termino, a => a.Titulo);
                var artistasTask = _repoArtista.BuscarTextoAsync(termino, a => a.NombreArtistico, a => a.Nombre, a => a.Apellido);
                var playlistsTask = _repoPlaylist.BuscarTextoAsync(termino, p => p.Nombre);
                var generosTask = _repoGenero.BuscarTextoAsync(termino, g => g.Nombre);

                await Task.WhenAll(cancionesTask, albumesTask, artistasTask, playlistsTask, generosTask);

                var resultado = new ResultadoBusqueda
                {
                    Canciones = (await cancionesTask).Take(limite),
                    Albumes = (await albumesTask).Take(limite),
                    Artistas = (await artistasTask).Take(limite),
                    Playlists = (await playlistsTask).Take(limite),
                    Generos = (await generosTask).Take(limite)
                };

                _logger.LogInformation("Búsqueda completada: {TotalResultados} resultados para '{Termino}'", 
                    resultado.TotalResultados, termino);
                
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la búsqueda global del término: {Termino}", termino);
                throw;
            }
        }, TimeSpan.FromMinutes(5));
    }

    public async Task<ResultadoBusqueda> BuscarAvanzadoAsync(FiltroBusqueda filtro, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Realizando búsqueda avanzada con filtros: {@Filtros}", filtro);

        try
        {
            var query = ConstruirQueryAvanzada(filtro);
            var parameters = ConstruirParametros(filtro);

            // Ejecutar consulta avanzada (implementación específica según tu ORM/DB)
            // Esta es una implementación simplificada
            var canciones = await _repoCancion.BuscarAsync(c => 
                (string.IsNullOrEmpty(filtro.Termino) || c.Titulo.Contains(filtro.Termino)) &&
                (!filtro.IdGenero.HasValue || c.Genero.idGenero == filtro.IdGenero.Value) &&
                (!filtro.IdArtista.HasValue || c.Artista.idArtista == filtro.IdArtista.Value),
                cancellationToken);

            var resultado = new ResultadoBusqueda
            {
                Canciones = canciones.Take(filtro.TamañoPagina),
                Albumes = new List<Album>(),
                Artistas = new List<Artista>(),
                Playlists = new List<PlayList>(),
                Generos = new List<Genero>()
            };

            _logger.LogDebug("Búsqueda avanzada completada: {Count} resultados", resultado.TotalResultados);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la búsqueda avanzada con filtros: {@Filtros}", filtro);
            throw;
        }
    }

    private string ConstruirQueryAvanzada(FiltroBusqueda filtro)
    {
        // Implementar lógica para construir consulta SQL avanzada
        // Esto depende de tu proveedor de base de datos
        return "SELECT * FROM Canciones WHERE 1=1"; // Placeholder
    }

    private DynamicParameters ConstruirParametros(FiltroBusqueda filtro)
    {
        var parameters = new DynamicParameters();
        
        if (!string.IsNullOrEmpty(filtro.Termino))
            parameters.Add("termino", $"%{filtro.Termino}%");
        
        if (filtro.IdGenero.HasValue)
            parameters.Add("idGenero", filtro.IdGenero.Value);
        
        // Agregar más parámetros según necesidad
        
        return parameters;
    }
}

public class ServicioRecomendaciones : IServicioRecomendaciones
{
    private readonly IRepoReproduccion _repoReproduccion;
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoGenero _repoGenero;
    private readonly IRepoArtista _repoArtista;
    private readonly ILogger<ServicioRecomendaciones> _logger;
    private readonly ICacheService _cacheService;

    public ServicioRecomendaciones(
        IRepoReproduccion repoReproduccion,
        IRepoCancion repoCancion,
        IRepoAlbum repoAlbum,
        IRepoGenero repoGenero,
        IRepoArtista repoArtista,
        ILogger<ServicioRecomendaciones> logger,
        ICacheService cacheService)
    {
        _repoReproduccion = repoReproduccion;
        _repoCancion = repoCancion;
        _repoAlbum = repoAlbum;
        _repoGenero = repoGenero;
        _repoArtista = repoArtista;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<Cancion>> ObtenerRecomendacionesPorUsuarioAsync(uint idUsuario, int limite = 10, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"recomendaciones_usuario_{idUsuario}_{limite}";
        
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogInformation("Generando recomendaciones para usuario: {UsuarioId}", idUsuario);

            // Obtener historial del usuario
            var historial = await _repoReproduccion.ObtenerHistorialUsuarioAsync(idUsuario, 100, cancellationToken);
            var historialList = historial.ToList();

            if (!historialList.Any())
            {
                _logger.LogDebug("Usuario {UsuarioId} sin historial, devolviendo canciones populares", idUsuario);
                return await _repoCancion.ObtenerCancionesPopularesAsync(limite, cancellationToken);
            }

            // Analizar preferencias del usuario
            var generosFavoritos = historialList
                .GroupBy(r => r.Cancion.Genero.idGenero)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            var artistasFavoritos = historialList
                .GroupBy(r => r.Cancion.Artista.idArtista)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            var cancionesEscuchadas = new HashSet<uint>(historialList.Select(r => r.Cancion.idCancion));

            // Generar recomendaciones basadas en preferencias
            var recomendaciones = new List<Cancion>();

            // Recomendar por géneros favoritos
            foreach (var idGenero in generosFavoritos)
            {
                var cancionesDelGenero = await _repoCancion.ObtenerPorGeneroAsync(idGenero, cancellationToken);
                var cancionesNoEscuchadas = cancionesDelGenero.Where(c => !cancionesEscuchadas.Contains(c.idCancion));
                recomendaciones.AddRange(cancionesNoEscuchadas);
            }

            // Recomendar por artistas favoritos
            foreach (var idArtista in artistasFavoritos)
            {
                var cancionesDelArtista = await _repoCancion.ObtenerPorArtistaAsync(idArtista, cancellationToken);
                var cancionesNoEscuchadas = cancionesDelArtista.Where(c => !cancionesEscuchadas.Contains(c.idCancion));
                recomendaciones.AddRange(cancionesNoEscuchadas);
            }

            // Si no hay suficientes recomendaciones, agregar canciones populares
            if (recomendaciones.Count < limite)
            {
                var cancionesPopulares = await _repoCancion.ObtenerCancionesPopularesAsync(limite, cancellationToken);
                var cancionesAdicionales = cancionesPopulares
                    .Where(c => !cancionesEscuchadas.Contains(c.idCancion) && !recomendaciones.Any(r => r.idCancion == c.idCancion))
                    .Take(limite - recomendaciones.Count);
                
                recomendaciones.AddRange(cancionesAdicionales);
            }

            // Eliminar duplicados y limitar resultados
            var resultado = recomendaciones
                .GroupBy(c => c.idCancion)
                .Select(g => g.First())
                .Take(limite)
                .ToList();

            _logger.LogInformation("Generadas {Count} recomendaciones para usuario {UsuarioId}", resultado.Count, idUsuario);
            return resultado;
        }, TimeSpan.FromMinutes(15));
    }

    public async Task<IEnumerable<Cancion>> ObtenerRecomendacionesPorGeneroAsync(byte idGenero, int limite = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Obteniendo recomendaciones por género: {GeneroId}", idGenero);

        var canciones = await _repoCancion.ObtenerPorGeneroAsync(idGenero, cancellationToken);
        var cancionesPopulares = canciones
            .OrderByDescending(c => c.TotalReproducciones)
            .Take(limite)
            .ToList();

        _logger.LogDebug("Encontradas {Count} canciones para género {GeneroId}", cancionesPopulares.Count, idGenero);
        return cancionesPopulares;
    }

    public async Task<IEnumerable<Cancion>> ObtenerRecomendacionesPorArtistaAsync(uint idArtista, int limite = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Obteniendo recomendaciones por artista: {ArtistaId}", idArtista);

        var canciones = await _repoCancion.ObtenerPorArtistaAsync(idArtista, cancellationToken);
        var cancionesPopulares = canciones
            .OrderByDescending(c => c.TotalReproducciones)
            .Take(limite)
            .ToList();

        _logger.LogDebug("Encontradas {Count} canciones para artista {ArtistaId}", cancionesPopulares.Count, idArtista);
        return cancionesPopulares;
    }

    public async Task<IEnumerable<Album>> ObtenerAlbumesRecomendadosAsync(uint idUsuario, int limite = 5, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"recomendaciones_albumes_{idUsuario}_{limite}";
        
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogDebug("Generando recomendaciones de álbumes para usuario: {UsuarioId}", idUsuario);

            // Obtener artistas favoritos del usuario
            var historial = await _repoReproduccion.ObtenerHistorialUsuarioAsync(idUsuario, 50, cancellationToken);
            var artistasFavoritos = historial
                .GroupBy(r => r.Cancion.Artista.idArtista)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            var albumesRecomendados = new List<Album>();

            // Recomendar álbumes de artistas favoritos
            foreach (var idArtista in artistasFavoritos)
            {
                var albumes = await _repoAlbum.ObtenerPorArtistaAsync(idArtista, cancellationToken);
                albumesRecomendados.AddRange(albumes);
            }

            // Si no hay suficientes, agregar álbumes recientes
            if (albumesRecomendados.Count < limite)
            {
                var albumesRecientes = await _repoAlbum.ObtenerAlbumesRecientesAsync(limite, cancellationToken);
                var albumesAdicionales = albumesRecientes
                    .Where(a => !albumesRecomendados.Any(ar => ar.idAlbum == a.idAlbum))
                    .Take(limite - albumesRecomendados.Count);
                
                albumesRecomendados.AddRange(albumesAdicionales);
            }

            var resultado = albumesRecomendados.Take(limite).ToList();
            _logger.LogDebug("Generadas {Count} recomendaciones de álbumes para usuario {UsuarioId}", resultado.Count, idUsuario);
            return resultado;
        }, TimeSpan.FromMinutes(20));
    }
}

namespace Spotify.Core.Services;

public interface ICacheService
{
    T? Get<T>(string key);
    Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    void Set<T>(string key, T value, TimeSpan? expiration = null);
    void Remove(string key);
    void Clear();
}

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, bool> _cacheKeys;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
        _cacheKeys = new ConcurrentDictionary<string, bool>();
    }

    public T? Get<T>(string key)
    {
        return _cache.TryGetValue(key, out T? value) ? value : default;
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out T? cachedValue))
        {
            return cachedValue;
        }

        var value = await factory();
        if (value != null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(1) // Tamaño relativo
                .SetPriority(CacheItemPriority.High);

            if (expiration.HasValue)
            {
                cacheEntryOptions.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Default 30 min
            }

            _cache.Set(key, value, cacheEntryOptions);
            _cacheKeys.TryAdd(key, true);
        }

        return value;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSize(1)
            .SetPriority(CacheItemPriority.High);

        if (expiration.HasValue)
        {
            cacheEntryOptions.SetAbsoluteExpiration(expiration.Value);
        }

        _cache.Set(key, value, cacheEntryOptions);
        _cacheKeys.TryAdd(key, true);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _cacheKeys.TryRemove(key, out _);
    }

    public void Clear()
    {
        foreach (var key in _cacheKeys.Keys)
        {
            _cache.Remove(key);
        }
        _cacheKeys.Clear();
    }
}

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

public interface IFileCleanupService
{
    Task CleanupOrphanedFilesAsync();
    Task CleanupTempFilesAsync();
}

public class FileCleanupService : IFileCleanupService, IHostedService, IDisposable
{
    private readonly ILogger<FileCleanupService> _logger;
    private readonly IServiceProvider _services;
    private readonly FileService _fileService;
    private Timer? _timer;

    public FileCleanupService(ILogger<FileCleanupService> logger, IServiceProvider services, FileService fileService)
    {
        _logger = logger;
        _services = services;
        _fileService = fileService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("File Cleanup Service started.");
        
        // Ejecutar limpieza cada 24 horas
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24));
        
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        try
        {
            _logger.LogInformation("Starting scheduled file cleanup...");
            
            await CleanupOrphanedFilesAsync();
            await CleanupTempFilesAsync();
            
            _logger.LogInformation("Scheduled file cleanup completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during file cleanup.");
        }
    }

    public async Task CleanupOrphanedFilesAsync()
    {
        try
        {
            using var scope = _services.CreateScope();
            
            // Aquí integrarías con los repositorios para encontrar archivos huérfanos
            // Por ahora, solo log
            _logger.LogInformation("Cleaning up orphaned files...");
            
            await Task.Delay(1000); // Simulación
            
            _logger.LogInformation("Orphaned files cleanup completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up orphaned files.");
        }
    }

    public async Task CleanupTempFilesAsync()
    {
        try
        {
            var tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "temp");
            if (Directory.Exists(tempPath))
            {
                var tempFiles = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => File.GetCreationTime(f) < DateTime.Now.AddHours(-24));
                
                foreach (var file in tempFiles)
                {
                    File.Delete(file);
                    _logger.LogInformation("Deleted temp file: {FilePath}", file);
                }
            }
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up temp files.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        _logger.LogInformation("File Cleanup Service stopped.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

public interface IFileService
{
    Task<string> GuardarPortadaAsync(IFormFile archivo);
    Task<string> GuardarCancionAsync(IFormFile archivo);
    void EliminarArchivo(string nombreArchivo, string tipo);
    string ObtenerRutaArchivo(string nombreArchivo, string tipo);
    bool ArchivoExiste(string nombreArchivo, string tipo);
    Task<FileInfo> ObtenerInfoArchivoAsync(string nombreArchivo, string tipo);
}

public class FileService : IFileService, IDisposable
{
    private readonly string _uploadPath;
    private readonly ILogger<FileService> _logger;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public FileService(string uploadPath, ILogger<FileService> logger)
    {
        _uploadPath = uploadPath;
        _logger = logger;
        
        // Crear directorios si no existen
        Directory.CreateDirectory(Path.Combine(_uploadPath, "portadas"));
        Directory.CreateDirectory(Path.Combine(_uploadPath, "canciones"));
        Directory.CreateDirectory(Path.Combine(_uploadPath, "temp"));
        
        _logger.LogInformation("FileService inicializado. Ruta: {UploadPath}", _uploadPath);
    }

    public async Task<string> GuardarPortadaAsync(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return "default_album.png";

        await _fileLock.WaitAsync();
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Validaciones
            await ValidarArchivoAsync(archivo, new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" }, 5 * 1024 * 1024);
            
            // Generar nombre único
            var extension = Path.GetExtension(archivo.FileName).ToLower();
            var nombreUnico = $"{Guid.NewGuid()}{extension}";
            var ruta = Path.Combine(_uploadPath, "portadas", nombreUnico);

            // Guardar archivo
            using (var stream = new FileStream(ruta, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await archivo.CopyToAsync(stream);
            }

            stopwatch.Stop();
            _logger.LogInformation("Portada guardada: {Nombre} ({Tamaño} bytes) en {Tiempo}ms", 
                nombreUnico, archivo.Length, stopwatch.ElapsedMilliseconds);
            
            return nombreUnico;
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task<string> GuardarCancionAsync(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            throw new ArgumentException("El archivo de la canción no puede estar vacío");

        await _fileLock.WaitAsync();
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Validaciones
            await ValidarArchivoAsync(archivo, new[] { ".mp3", ".wav", ".ogg", ".flac" }, 50 * 1024 * 1024);
            
            // Generar nombre único
            var extension = Path.GetExtension(archivo.FileName).ToLower();
            var nombreUnico = $"{Guid.NewGuid()}{extension}";
            var ruta = Path.Combine(_uploadPath, "canciones", nombreUnico);

            // Guardar archivo
            using (var stream = new FileStream(ruta, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await archivo.CopyToAsync(stream);
            }

            stopwatch.Stop();
            _logger.LogInformation("Canción guardada: {Nombre} ({Tamaño} bytes) en {Tiempo}ms", 
                nombreUnico, archivo.Length, stopwatch.ElapsedMilliseconds);
            
            return nombreUnico;
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private async Task ValidarArchivoAsync(IFormFile archivo, string[] extensionesPermitidas, long tamañoMaximo)
    {
        var extension = Path.GetExtension(archivo.FileName).ToLower();
        
        if (!extensionesPermitidas.Contains(extension))
            throw new ArgumentException($"Solo se permiten archivos: {string.Join(", ", extensionesPermitidas)}");

        if (archivo.Length > tamañoMaximo)
            throw new ArgumentException($"El archivo no puede ser mayor a {tamañoMaximo / (1024 * 1024)}MB");

        // Validación adicional: leer primeros bytes para verificar tipo real
        await using var memoryStream = new MemoryStream();
        await archivo.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        
        // Aquí podrías agregar validaciones más específicas del tipo de archivo
    }

    public void EliminarArchivo(string nombreArchivo, string tipo)
    {
        if (string.IsNullOrEmpty(nombreArchivo) || nombreArchivo == "default_album.png")
            return;

        var ruta = Path.Combine(_uploadPath, tipo, nombreArchivo);
        if (File.Exists(ruta))
        {
            try
            {
                File.Delete(ruta);
                _logger.LogInformation("Archivo eliminado: {Ruta}", ruta);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo eliminar el archivo: {Ruta}", ruta);
            }
        }
    }

    public string ObtenerRutaArchivo(string nombreArchivo, string tipo)
    {
        if (string.IsNullOrEmpty(nombreArchivo))
            return string.Empty;

        return $"/uploads/{tipo}/{nombreArchivo}";
    }

    public bool ArchivoExiste(string nombreArchivo, string tipo)
    {
        if (string.IsNullOrEmpty(nombreArchivo))
            return false;

        var ruta = Path.Combine(_uploadPath, tipo, nombreArchivo);
        return File.Exists(ruta);
    }

    public async Task<FileInfo> ObtenerInfoArchivoAsync(string nombreArchivo, string tipo)
    {
        if (string.IsNullOrEmpty(nombreArchivo) || !ArchivoExiste(nombreArchivo, tipo))
            throw new FileNotFoundException("Archivo no encontrado");

        var ruta = Path.Combine(_uploadPath, tipo, nombreArchivo);
        var fileInfo = new FileInfo(ruta);
        
        // Verificar integridad del archivo
        if (fileInfo.Length == 0)
        {
            _logger.LogWarning("Archivo corrupto detectado: {Ruta}", ruta);
            throw new InvalidDataException("El archivo está corrupto");
        }

        return await Task.FromResult(fileInfo);
    }

    public void Dispose()
    {
        _fileLock?.Dispose();
    }

    internal string GetUploadPath()
    {
        throw new NotImplementedException();
    }
}

public interface IRateLimitService
{
    bool IsAllowed(string endpoint, string identifier, int maxRequests, TimeSpan timeWindow);
    (bool allowed, int remaining, TimeSpan resetAfter) GetRateLimitInfo(string endpoint, string identifier, int maxRequests, TimeSpan timeWindow);
}

public class RateLimitService : IRateLimitService
{
    private readonly ConcurrentDictionary<string, ClientRateLimit> _rateLimits = new();

    public bool IsAllowed(string endpoint, string identifier, int maxRequests, TimeSpan timeWindow)
    {
        var key = $"{endpoint}:{identifier}";
        var now = DateTime.UtcNow;

        if (!_rateLimits.TryGetValue(key, out var rateLimit))
        {
            rateLimit = new ClientRateLimit
            {
                Endpoint = endpoint,
                Identifier = identifier,
                MaxRequests = maxRequests,
                TimeWindow = timeWindow,
                Requests = new List<DateTime> { now }
            };
            _rateLimits[key] = rateLimit;
            return true;
        }

        // Limpiar requests antiguos
        rateLimit.Requests.RemoveAll(r => r < now - timeWindow);

        if (rateLimit.Requests.Count >= maxRequests)
        {
            return false;
        }

        rateLimit.Requests.Add(now);
        return true;
    }

    public (bool allowed, int remaining, TimeSpan resetAfter) GetRateLimitInfo(
        string endpoint, string identifier, int maxRequests, TimeSpan timeWindow)
    {
        var key = $"{endpoint}:{identifier}";
        var now = DateTime.UtcNow;

        if (!_rateLimits.TryGetValue(key, out var rateLimit))
        {
            return (true, maxRequests, timeWindow);
        }

        rateLimit.Requests.RemoveAll(r => r < now - timeWindow);
        var remaining = Math.Max(0, maxRequests - rateLimit.Requests.Count);
        var oldestRequest = rateLimit.Requests.Min();
        var resetAfter = timeWindow - (now - oldestRequest);

        return (rateLimit.Requests.Count < maxRequests, remaining, resetAfter);
    }

    // Limpieza periódica de rate limits expirados
    public void CleanupExpiredRateLimits()
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _rateLimits
            .Where(kvp => kvp.Value.Requests.All(r => r < now - kvp.Value.TimeWindow))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _rateLimits.TryRemove(key, out _);
        }
    }

    private class ClientRateLimit
    {
        public string Endpoint { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public int MaxRequests { get; set; }
        public TimeSpan TimeWindow { get; set; }
        public List<DateTime> Requests { get; set; } = new List<DateTime>();
    }
}

namespace Spotify.Core.Persistencia.Contracts;

public interface IServicioAnaliticas
{
    Task<EstadisticasUsuario> ObtenerEstadisticasUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default);
    Task<EstadisticasGlobales> ObtenerEstadisticasGlobalesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TopArtista>> ObtenerTopArtistasAsync(DateTime desde, DateTime hasta, int limite = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<TopCancion>> ObtenerTopCancionesAsync(DateTime desde, DateTime hasta, int limite = 10, CancellationToken cancellationToken = default);
}

public interface IServicioBusqueda
{
    Task<ResultadoBusqueda> BuscarGlobalAsync(string termino, int limite = 20, CancellationToken cancellationToken = default);
    Task<ResultadoBusqueda> BuscarAvanzadoAsync(FiltroBusqueda filtro, CancellationToken cancellationToken = default);
}

public interface IServicioRecomendaciones
{
    Task<IEnumerable<Cancion>> ObtenerRecomendacionesPorUsuarioAsync(uint idUsuario, int limite = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cancion>> ObtenerRecomendacionesPorGeneroAsync(byte idGenero, int limite = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cancion>> ObtenerRecomendacionesPorArtistaAsync(uint idArtista, int limite = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<Album>> ObtenerAlbumesRecomendadosAsync(uint idUsuario, int limite = 5, CancellationToken cancellationToken = default);
}

namespace Spotify.Core.Persistencia;

public interface IRepoAlbum : IRepoBase<Album>, IRepoBusqueda<Album>
{
    // Operaciones específicas de Album
    IEnumerable<Album> ObtenerAlbumesRecientes(int limite = 10);
    Task<IEnumerable<Album>> ObtenerAlbumesRecientesAsync(int limite = 10, CancellationToken cancellationToken = default);
        
    IEnumerable<Album> ObtenerPorArtista(uint idArtista);
    Task<IEnumerable<Album>> ObtenerPorArtistaAsync(uint idArtista, CancellationToken cancellationToken = default);
        
    IEnumerable<Album> ObtenerConCanciones();
    Task<IEnumerable<Album>> ObtenerConCancionesAsync(CancellationToken cancellationToken = default);
        
    Album? ObtenerConArtistaYCanciones(uint idAlbum);
    Task<Album?> ObtenerConArtistaYCancionesAsync(uint idAlbum, CancellationToken cancellationToken = default);
}

public interface IRepoArtista : IRepoBase<Artista>, IRepoBusqueda<Artista>
{
    // Operaciones específicas de Artista
    IEnumerable<Artista> ObtenerArtistasPopulares(int limite = 10);
    Task<IEnumerable<Artista>> ObtenerArtistasPopularesAsync(int limite = 10, CancellationToken cancellationToken = default);
        
    IEnumerable<Artista> ObtenerConAlbumes();
    Task<IEnumerable<Artista>> ObtenerConAlbumesAsync(CancellationToken cancellationToken = default);
        
    Artista? ObtenerPorNombreArtistico(string nombreArtistico);
    Task<Artista?> ObtenerPorNombreArtisticoAsync(string nombreArtistico, CancellationToken cancellationToken = default);
}

public interface IRepoBase<T> where T : class
{
    // Operaciones síncronas
    T? ObtenerPorId(object id);
    IEnumerable<T> ObtenerTodos();
    IEnumerable<T> Buscar(Expression<Func<T, bool>> predicado);
    void Insertar(T entidad);
    void Actualizar(T entidad);
    void Eliminar(object id);
    void Eliminar(T entidad);
    
    // Operaciones asíncronas
    Task<T?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado, CancellationToken cancellationToken = default);
    Task InsertarAsync(T entidad, CancellationToken cancellationToken = default);
    Task ActualizarAsync(T entidad, CancellationToken cancellationToken = default);
    Task EliminarAsync(object id, CancellationToken cancellationToken = default);
    
    // Operaciones avanzadas
    int Contar();
    Task<int> ContarAsync(CancellationToken cancellationToken = default);
    bool Existe(object id);
    Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default);
}

public interface IRepoBusqueda<T> where T : class
{
    IEnumerable<T> BuscarTexto(string termino, params Expression<Func<T, object>>[] propiedades);
    Task<IEnumerable<T>> BuscarTextoAsync(string termino, params Expression<Func<T, object>>[] propiedades);
        
    IEnumerable<T> ObtenerPaginado(int pagina, int tamañoPagina, string ordenarPor = "Id");
    Task<IEnumerable<T>> ObtenerPaginadoAsync(int pagina, int tamañoPagina, string ordenarPor = "Id", CancellationToken cancellationToken = default);
        
    IEnumerable<T> ObtenerConRelaciones(params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> ObtenerConRelacionesAsync(params Expression<Func<T, object>>[] includes);
}

public interface IRepoCancion : IRepoBase<Cancion>, IRepoBusqueda<Cancion>
{
        // Operaciones específicas de Cancion
    IEnumerable<Cancion> ObtenerCancionesPopulares(int limite = 10);
    Task<IEnumerable<Cancion>> ObtenerCancionesPopularesAsync(int limite = 10, CancellationToken cancellationToken = default);
        
    IEnumerable<Cancion> ObtenerPorAlbum(uint idAlbum);
    Task<IEnumerable<Cancion>> ObtenerPorAlbumAsync(uint idAlbum, CancellationToken cancellationToken = default);
        
    IEnumerable<Cancion> ObtenerPorArtista(uint idArtista);
    Task<IEnumerable<Cancion>> ObtenerPorArtistaAsync(uint idArtista, CancellationToken cancellationToken = default);
        
    IEnumerable<Cancion> ObtenerPorGenero(byte idGenero);
    Task<IEnumerable<Cancion>> ObtenerPorGeneroAsync(byte idGenero, CancellationToken cancellationToken = default);
        
    IEnumerable<Cancion> BuscarPorLetra(string texto);
    Task<IEnumerable<Cancion>> BuscarPorLetraAsync(string texto, CancellationToken cancellationToken = default);
        
    Cancion? ObtenerConAlbumYArtista(uint idCancion);
    Task<Cancion?> ObtenerConAlbumYArtistaAsync(uint idCancion, CancellationToken cancellationToken = default);
        
    Task<int> IncrementarReproduccionesAsync(uint idCancion, CancellationToken cancellationToken = default);
}

public interface IRepoGenero : IRepoBase<Genero>
{
        // Operaciones específicas de Genero
    IEnumerable<Genero> ObtenerGenerosPopulares();
    Task<IEnumerable<Genero>> ObtenerGenerosPopularesAsync(CancellationToken cancellationToken = default);
        
    Genero? ObtenerPorNombre(string nombre);
    Task<Genero?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default);
}

public interface IRepoNacionalidad : IRepoBase<Nacionalidad>
{
        // Operaciones específicas de Nacionalidad
    Nacionalidad? ObtenerPorPais(string pais);
    Task<Nacionalidad?> ObtenerPorPaisAsync(string pais, CancellationToken cancellationToken = default);
        
    IEnumerable<Nacionalidad> ObtenerPaisesConUsuarios();
    Task<IEnumerable<Nacionalidad>> ObtenerPaisesConUsuariosAsync(CancellationToken cancellationToken = default);
}

public interface IRepoNacionalidad : IRepoBase<Nacionalidad>
{
        // Operaciones específicas de Nacionalidad
    Nacionalidad? ObtenerPorPais(string pais);
    Task<Nacionalidad?> ObtenerPorPaisAsync(string pais, CancellationToken cancellationToken = default);
        
    IEnumerable<Nacionalidad> ObtenerPaisesConUsuarios();
    Task<IEnumerable<Nacionalidad>> ObtenerPaisesConUsuariosAsync(CancellationToken cancellationToken = default);
}

public interface IRepoPlaylist : IRepoBase<PlayList>
{
        // Operaciones específicas de Playlist
    IEnumerable<PlayList> ObtenerPorUsuario(uint idUsuario);
    Task<IEnumerable<PlayList>> ObtenerPorUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default);
        
    PlayList? ObtenerConCanciones(uint idPlaylist);
    Task<PlayList?> ObtenerConCancionesAsync(uint idPlaylist, CancellationToken cancellationToken = default);
        
    Task<bool> AgregarCancionAsync(uint idPlaylist, uint idCancion, CancellationToken cancellationToken = default);
    Task<bool> RemoverCancionAsync(uint idPlaylist, uint idCancion, CancellationToken cancellationToken = default);
    Task<bool> ReordenarCancionesAsync(uint idPlaylist, List<uint> idsCanciones, CancellationToken cancellationToken = default);
        
    Task<int> ObtenerTotalCancionesAsync(uint idPlaylist, CancellationToken cancellationToken = default);
    Task<TimeSpan> ObtenerDuracionTotalAsync(uint idPlaylist, CancellationToken cancellationToken = default);
}

public interface IRepoRegistro : IRepoBase<Registro>
{
        // Operaciones específicas de Registro
    Registro? ObtenerSuscripcionActiva(uint idUsuario);
    Task<Registro?> ObtenerSuscripcionActivaAsync(uint idUsuario, CancellationToken cancellationToken = default);
        
    IEnumerable<Registro> ObtenerSuscripcionesPorUsuario(uint idUsuario);
    Task<IEnumerable<Registro>> ObtenerSuscripcionesPorUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default);
        
    IEnumerable<Registro> ObtenerSuscripcionesExpiradas();
    Task<IEnumerable<Registro>> ObtenerSuscripcionesExpiradasAsync(CancellationToken cancellationToken = default);
        
    IEnumerable<Registro> ObtenerSuscripcionesPorExpirar(int dias = 7);
    Task<IEnumerable<Registro>> ObtenerSuscripcionesPorExpirarAsync(int dias = 7, CancellationToken cancellationToken = default);
        
    Task<bool> RenovarSuscripcionAsync(uint idSuscripcion, CancellationToken cancellationToken = default);
    Task<bool> CancelarSuscripcionAsync(uint idSuscripcion, CancellationToken cancellationToken = default);
}

public interface IRepoReproduccion : IRepoBase<Reproduccion>
{
        // Operaciones específicas de Reproduccion
    IEnumerable<Reproduccion> ObtenerHistorialUsuario(uint idUsuario, int limite = 50);
    Task<IEnumerable<Reproduccion>> ObtenerHistorialUsuarioAsync(uint idUsuario, int limite = 50, CancellationToken cancellationToken = default);
        
    IEnumerable<Reproduccion> ObtenerRecientes(DateTime desde);
    Task<IEnumerable<Reproduccion>> ObtenerRecientesAsync(DateTime desde, CancellationToken cancellationToken = default);
        
    Task<bool> RegistrarReproduccionAsync(uint idUsuario, uint idCancion, TimeSpan progreso, bool completa, string? dispositivo, CancellationToken cancellationToken = default);
        
    IEnumerable<Cancion> ObtenerCancionesMasEscuchadas(uint idUsuario, int limite = 10);
    Task<IEnumerable<Cancion>> ObtenerCancionesMasEscuchadasAsync(uint idUsuario, int limite = 10, CancellationToken cancellationToken = default);
        
    Task<int> ObtenerTotalReproduccionesAsync(uint idUsuario, CancellationToken cancellationToken = default);
    Task<TimeSpan> ObtenerTiempoTotalEscuchadoAsync(uint idUsuario, CancellationToken cancellationToken = default);
}

public interface IRepoTipoSuscripcion : IRepoBase<TipoSuscripcion>
{
        // Operaciones específicas de TipoSuscripcion
    TipoSuscripcion? ObtenerMasPopular();
    Task<TipoSuscripcion?> ObtenerMasPopularAsync(CancellationToken cancellationToken = default);
        
    IEnumerable<TipoSuscripcion> ObtenerOrdenadosPorPrecio();
    Task<IEnumerable<TipoSuscripcion>> ObtenerOrdenadosPorPrecioAsync(CancellationToken cancellationToken = default);
}

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