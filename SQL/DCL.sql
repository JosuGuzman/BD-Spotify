USE 5to_Spotify;

-- Creación de usuarios con permisos específicos
DROP USER IF EXISTS 'AdminSpotify'@'localhost';
DROP USER IF EXISTS 'UsuarioApp'@'localhost';
DROP USER IF EXISTS 'GestorContenido'@'localhost';
DROP USER IF EXISTS 'Reportes'@'localhost';

-- 1. Usuario Administrador (acceso completo)
CREATE USER 'AdminSpotify'@'localhost' IDENTIFIED BY 'AdminSecure123!';
GRANT ALL PRIVILEGES ON 5to_Spotify.* TO 'AdminSpotify'@'localhost';
GRANT EXECUTE ON PROCEDURE 5to_Spotify.* TO 'AdminSpotify'@'localhost';

-- 2. Usuario de la Aplicación (para conexión ASP.NET)
CREATE USER 'UsuarioApp'@'localhost' IDENTIFIED BY 'AppSecure456!';
GRANT SELECT, INSERT, UPDATE, DELETE ON 5to_Spotify.* TO 'UsuarioApp'@'localhost';
GRANT EXECUTE ON PROCEDURE 5to_Spotify.* TO 'UsuarioApp'@'localhost';

-- 3. Usuario Gestor de Contenido (solo CRUD de contenido)
CREATE USER 'GestorContenido'@'localhost' IDENTIFIED BY 'GestorSecure789!';
GRANT SELECT, INSERT, UPDATE ON 5to_Spotify.Artista TO 'GestorContenido'@'localhost';
GRANT SELECT, INSERT, UPDATE ON 5to_Spotify.Album TO 'GestorContenido'@'localhost';
GRANT SELECT, INSERT, UPDATE ON 5to_Spotify.Cancion TO 'GestorContenido'@'localhost';
GRANT SELECT, INSERT, UPDATE ON 5to_Spotify.Genero TO 'GestorContenido'@'localhost';
GRANT SELECT, INSERT, UPDATE ON 5to_Spotify.Nacionalidad TO 'GestorContenido'@'localhost';
GRANT SELECT ON 5to_Spotify.* TO 'GestorContenido'@'localhost';

-- 4. Usuario de Reportes (solo lecturas)
CREATE USER 'Reportes'@'localhost' IDENTIFIED BY 'ReportSecure321!';
GRANT SELECT ON 5to_Spotify.VistaDashboardAdmin TO 'Reportes'@'localhost';
GRANT SELECT ON 5to_Spotify.VistaEstadisticasDiarias TO 'Reportes'@'localhost';
GRANT SELECT ON 5to_Spotify.VistaUsuariosSuscripciones TO 'Reportes'@'localhost';
GRANT SELECT ON 5to_Spotify.VistaCancionesDetalladas TO 'Reportes'@'localhost';
GRANT EXECUTE ON PROCEDURE 5to_Spotify.ObtenerEstadisticasSistema TO 'Reportes'@'localhost';

-- Revocar permisos peligrosos de usuarios no admin
REVOKE DROP, CREATE USER, GRANT OPTION FROM 'UsuarioApp'@'localhost';
REVOKE DROP, CREATE USER, GRANT OPTION FROM 'GestorContenido'@'localhost';
REVOKE DROP, CREATE USER, GRANT OPTION FROM 'Reportes'@'localhost';

-- Aplicar cambios
FLUSH PRIVILEGES;

-- Verificación de permisos
SELECT '✅ Usuarios y permisos configurados correctamente' AS Mensaje;