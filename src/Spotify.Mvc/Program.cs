using Spotify.Core;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Connections;
using System.Net;
using Spotify.ReposDapper;
using Spotify.Core.Persistencia;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// Configurar sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Database Factory
builder.Services.AddScoped<IConnectionFactory>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("MySQL") 
        ?? throw new InvalidOperationException("Connection string not found");
    return new DbConnectionFactory(connectionString);
});

// Repositories
var connectionString = builder.Configuration.GetConnectionString("MySQL") 
    ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddScoped<IRepoAlbum>(p => new RepoAlbum(connectionString, p.GetRequiredService<ILogger<RepoAlbum>>()));
builder.Services.AddScoped<IRepoArtista>(p => new RepoArtista(connectionString, p.GetRequiredService<ILogger<RepoArtista>>()));
builder.Services.AddScoped<IRepoCancion>(p => new RepoCancion(connectionString, p.GetRequiredService<ILogger<RepoCancion>>()));
builder.Services.AddScoped<IRepoGenero>(p => new RepoGenero(connectionString, p.GetRequiredService<ILogger<RepoGenero>>()));
builder.Services.AddScoped<IRepoPlaylist>(p => new RepoPlaylist(connectionString, p.GetRequiredService<ILogger<RepoPlaylist>>()));
builder.Services.AddScoped<IRepoReproduccion>(p => new RepoReproduccion(connectionString, p.GetRequiredService<ILogger<RepoReproduccion>>()));
builder.Services.AddScoped<IRepoUsuario>(p => new RepoUsuario(connectionString, p.GetRequiredService<ILogger<RepoUsuario>>()));
builder.Services.AddScoped<IRepoNacionalidad>(p => new RepoNacionalidad(connectionString, p.GetRequiredService<ILogger<RepoNacionalidad>>()));
builder.Services.AddScoped<IRepoTipoSuscripcion>(p => new RepoTipoSuscripcion(connectionString, p.GetRequiredService<ILogger<RepoTipoSuscripcion>>()));
builder.Services.AddScoped<IRepoRegistro>(p => new RepoSuscripcion(connectionString, p.GetRequiredService<ILogger<RepoSuscripcion>>()));

// File Service
builder.Services.AddScoped<IFileService>(provider =>
{
    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
    var logger = provider.GetRequiredService<ILogger<FileService>>();
    return new FileService(uploadPath, logger);
});

// Configurar límites de subida de archivos
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

internal class DbConnectionFactory : IConnectionFactory
{
    public DbConnectionFactory(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }

    public ValueTask<ConnectionContext> ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}