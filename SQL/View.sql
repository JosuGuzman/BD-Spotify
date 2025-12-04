USE 5to_Spotify;

-- Vista: Dashboard del Administrador
CREATE OR REPLACE VIEW VistaDashboardAdmin AS
SELECT 
    -- Estadísticas de usuarios
    (SELECT COUNT(*) FROM Usuario WHERE EstaActivo = 1 AND IdRol = 2) AS TotalUsuariosRegistrados,
    (SELECT COUNT(*) FROM Usuario WHERE EstaActivo = 1 AND IdRol = 3) AS TotalAdministradores,
    (SELECT COUNT(*) FROM Usuario WHERE DATE(FechaRegistro) = CURDATE()) AS NuevosUsuariosHoy,
    
    -- Estadísticas de contenido
    (SELECT COUNT(*) FROM Cancion WHERE EstaActiva = 1) AS TotalCanciones,
    (SELECT COUNT(*) FROM Artista WHERE EstaActivo = 1) AS TotalArtistas,
    (SELECT COUNT(*) FROM Album WHERE EstaActivo = 1) AS TotalAlbumes,
    (SELECT COUNT(*) FROM Playlist WHERE EstaActiva = 1) AS TotalPlaylists,
    
    -- Estadísticas de reproducción
    (SELECT SUM(ContadorReproducciones) FROM Cancion) AS TotalReproducciones,
    (SELECT COUNT(*) FROM HistorialReproduccion WHERE DATE(FechaReproduccion) = CURDATE()) AS ReproduccionesHoy;

-- Vista: Canciones con detalles completos
CREATE OR REPLACE VIEW VistaCancionesDetalladas AS
SELECT 
    c.IdCancion,
    c.Titulo,
    c.DuracionSegundos,
    SEC_TO_TIME(c.DuracionSegundos) AS DuracionFormato,
    c.ArchivoMP3,
    c.ContadorReproducciones,
    c.EstaActiva,
    c.FechaCreacion,
    c.FechaActualizacion,
    
    -- Información del álbum
    al.IdAlbum,
    al.Titulo AS AlbumTitulo,
    al.Portada AS AlbumPortada,
    al.FechaLanzamiento,
    
    -- Información del artista
    a.IdArtista,
    a.NombreArtistico AS Artista,
    a.FotoArtista,
    a.NombreReal,
    a.ApellidoReal,
    
    -- Información del género
    g.IdGenero,
    g.Nombre AS Genero,
    g.Descripcion AS GeneroDescripcion
    
FROM Cancion c
JOIN Album al ON c.IdAlbum = al.IdAlbum AND al.EstaActivo = 1
JOIN Artista a ON c.IdArtista = a.IdArtista AND a.EstaActivo = 1
JOIN Genero g ON c.IdGenero = g.IdGenero AND g.EstaActivo = 1
WHERE c.EstaActiva = 1
ORDER BY c.ContadorReproducciones DESC;

-- Vista: Playlists Públicas con información
CREATE OR REPLACE VIEW VistaPlaylistsPublicas AS
SELECT 
    p.IdPlaylist,
    p.Nombre,
    p.Descripcion,
    p.IdUsuario,
    u.NombreUsuario AS Creador,
    u.FotoPerfil AS FotoCreador,
    p.EsPublica,
    p.FechaCreacion,
    p.FechaActualizacion,
    (SELECT COUNT(*) FROM Cancion_Playlist cp WHERE cp.IdPlaylist = p.IdPlaylist) AS TotalCanciones,
    (SELECT SUM(c.DuracionSegundos) 
     FROM Cancion_Playlist cp 
     JOIN Cancion c ON cp.IdCancion = c.IdCancion 
     WHERE cp.IdPlaylist = p.IdPlaylist) AS DuracionTotalSegundos,
    p.EstaActiva
FROM Playlist p
JOIN Usuario u ON p.IdUsuario = u.IdUsuario AND u.EstaActivo = 1
WHERE p.EsPublica = 1 
AND p.EstaActiva = 1
AND p.EsSistema = 0
ORDER BY p.FechaCreacion DESC;