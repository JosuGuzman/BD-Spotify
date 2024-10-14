-- Creación de usuarios
CREATE USER 'Admin'@'localhost' IDENTIFIED BY 'adiMn456?';
CREATE USER 'Admin'@'%' IDENTIFIED BY 'Adimer213#';
CREATE USER ''@'' IDENTIFIED BY 'artista456';
CREATE USER 'CreadorPlayList'@'localhost' IDENTIFIED BY 'playlist789';
CREATE USER 'reporte_viewer'@'localhost' IDENTIFIED BY 'reporte321';

-- Asignación de permisos para Admin
GRANT ALL PRIVILEGES ON 5to_Spotify.* TO 'Admin'@'localhost';
GRANT ALL PRIVILEGES ON 5to_Spotify.* TO 'Admin'@'%';

-- Asignación de permisos para Manager
GRANT SELECT, INSERT, UPDATE ON 5to_Spotify.Artista TO 'Manager'@'localhost';
GRANT SELECT, INSERT, UPDATE ON 5to_Spotify.Album TO 'Manager'@'localhost';
GRANT SELECT, INSERT, UPDATE ON 5to_Spotify.Cancion TO 'Manager'@'localhost';

-- Asignación de permisos para CreadorPlayList
GRANT SELECT ON 5to_Spotify.Cancion TO 'CreadorPlayList'@'localhost';
GRANT SELECT, INSERT, UPDATE, DELETE ON 5to_Spotify.Playlist TO 'CreadorPlayList'@'localhost';
GRANT SELECT, INSERT, UPDATE, DELETE ON 5to_Spotify.Playlist_Cancion TO 'CreadorPlayList'@'localhost';

-- Asignación de permisos para reporte_viewer
GRANT SELECT ON 5to_Spotify.HistorialReproducción TO 'reporte_viewer'@'localhost';
GRANT SELECT ON 5to_Spotify.Usuario TO 'reporte_viewer'@'localhost';
GRANT SELECT ON 5to_Spotify.Suscripcion TO 'reporte_viewer'@'localhost';

-- Aplicar los cambios
FLUSH PRIVILEGES;