using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Spotify.Core.Services;

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