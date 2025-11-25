using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Spotify.Core;

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