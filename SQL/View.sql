USE 5to_Spotify;

DELIMITER $$

CREATE OR REPLACE VIEW `VistaDashboardAdmin` AS
SELECT 
    COUNT(DISTINCT u.idUsuario) AS TotalUsuarios,
    COUNT(DISTINCT c.idCancion) AS TotalCanciones,
    COUNT(DISTINCT a.idArtista) AS TotalArtistas,
    COUNT(DISTINCT al.idAlbum) AS TotalAlbumes,
    SUM(CASE WHEN hr.FechaReproduccion >= CURDATE() 
             AND hr.FechaReproduccion < CURDATE() + INTERVAL 1 DAY
             THEN 1 ELSE 0 END) AS ReproduccionesHoy
FROM Usuario u
LEFT JOIN Cancion c ON c.EstaActiva = TRUE
LEFT JOIN Artista a ON a.EstaActivo = TRUE
LEFT JOIN Album al ON 1=1
LEFT JOIN HistorialReproduccion hr ON 1=1;

DELIMITER;

DELIMITER $$

CREATE OR REPLACE VIEW `VistaHistorialUsuario` AS
SELECT 
    hr.idUsuario,
    hr.FechaReproduccion,
    c.Titulo AS Cancion,
    c.Duracion,
    a.NombreArtistico AS Artista,
    al.Titulo AS Album
FROM HistorialReproduccion hr
JOIN Cancion c ON hr.idCancion = c.idCancion AND c.EstaActiva = TRUE
JOIN Artista a ON c.idArtista = a.idArtista AND a.EstaActivo = TRUE
JOIN Album al ON c.idAlbum = al.idAlbum
ORDER BY hr.idUsuario, hr.FechaReproduccion DESC;

DELIMITER;

DELIMITER $$

CREATE OR REPLACE VIEW `VistaPlaylistsPublicas` AS
SELECT 
    p.idPlaylist,
    p.Nombre,
    u.NombreUsuario AS Creador,
    COUNT(cp.idCancion) AS TotalCanciones
FROM Playlist p
JOIN Usuario u ON p.idUsuario = u.idUsuario AND u.EstaActivo = TRUE
LEFT JOIN Cancion_Playlist cp ON cp.idPlaylist = p.idPlaylist
WHERE p.EsPublica = TRUE
GROUP BY p.idPlaylist, p.Nombre, u.NombreUsuario;

DELIMITER;