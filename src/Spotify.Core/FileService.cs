using Microsoft.AspNetCore.Http;

namespace Spotify.Core;

public class FileService
{
    private readonly string _uploadPath;

    public FileService(string uploadPath)
    {
        _uploadPath = uploadPath;
        
        // Crear directorios si no existen
        Directory.CreateDirectory(Path.Combine(_uploadPath, "portadas"));
        Directory.CreateDirectory(Path.Combine(_uploadPath, "canciones"));
        
        Console.WriteLine($"FileService inicializado. Ruta de uploads: {_uploadPath}");
    }

    // Método para guardar portadas de álbumes
    public async Task<string> GuardarPortadaAsync(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return "default_album.png";

        // Validar que sea imagen
        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var extension = Path.GetExtension(archivo.FileName).ToLower();
        
        if (!extensionesPermitidas.Contains(extension))
            throw new ArgumentException("Solo se permiten archivos de imagen (JPG, JPEG, PNG, GIF, BMP)");

        // Validar tamaño (máximo 5MB)
        if (archivo.Length > 5 * 1024 * 1024)
            throw new ArgumentException("La imagen no puede ser mayor a 5MB");

        // Generar nombre único
        var nombreUnico = $"album_{Guid.NewGuid()}{extension}";
        var rutaCompleta = Path.Combine(_uploadPath, "portadas", nombreUnico);

        // Guardar archivo
        using var stream = new FileStream(rutaCompleta, FileMode.Create);
        await archivo.CopyToAsync(stream);

        Console.WriteLine($"Portada guardada: {nombreUnico}");
        return nombreUnico;
    }

    // Método para guardar archivos MP3
    public async Task<string> GuardarCancionAsync(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            throw new ArgumentException("El archivo de canción no puede estar vacío");

        // Validar que sea MP3
        var extension = Path.GetExtension(archivo.FileName).ToLower();
        if (extension != ".mp3")
            throw new ArgumentException("Solo se permiten archivos MP3");

        // Validar tamaño (máximo 20MB)
        if (archivo.Length > 20 * 1024 * 1024)
            throw new ArgumentException("El archivo MP3 no puede ser mayor a 20MB");

        // Generar nombre único
        var nombreUnico = $"cancion_{Guid.NewGuid()}{extension}";
        var rutaCompleta = Path.Combine(_uploadPath, "canciones", nombreUnico);

        // Guardar archivo
        using var stream = new FileStream(rutaCompleta, FileMode.Create);
        await archivo.CopyToAsync(stream);

        Console.WriteLine($"Canción MP3 guardada: {nombreUnico}");
        return nombreUnico;
    }

    // Método para eliminar archivos
    public void EliminarArchivo(string nombreArchivo, string tipo)
    {
        if (string.IsNullOrEmpty(nombreArchivo) || nombreArchivo == "default_album.png")
            return;

        var ruta = Path.Combine(_uploadPath, tipo, nombreArchivo);
        if (File.Exists(ruta))
        {
            File.Delete(ruta);
            Console.WriteLine($"Archivo eliminado: {ruta}");
        }
    }

    // Método para obtener la ruta pública del archivo
    public string ObtenerRutaArchivo(string nombreArchivo, string tipo)
    {
        if (string.IsNullOrEmpty(nombreArchivo))
            return string.Empty;

        return $"/uploads/{tipo}/{nombreArchivo}";
    }

    // Método para verificar si un archivo existe
    public bool ArchivoExiste(string nombreArchivo, string tipo)
    {
        if (string.IsNullOrEmpty(nombreArchivo))
            return false;

        var ruta = Path.Combine(_uploadPath, tipo, nombreArchivo);
        return File.Exists(ruta);
    }

    // Método para obtener el tamaño del archivo
    public long? ObtenerTamañoArchivo(string nombreArchivo, string tipo)
    {
        if (string.IsNullOrEmpty(nombreArchivo) || !ArchivoExiste(nombreArchivo, tipo))
            return null;

        var ruta = Path.Combine(_uploadPath, tipo, nombreArchivo);
        return new FileInfo(ruta).Length;
    }
}