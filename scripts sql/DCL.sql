-- Creación de usuarios
CREATE USER IF NOT EXISTS 'Admin'@'localhost' IDENTIFIED BY '7wQ0EgQ6$';
CREATE USER IF NOT EXISTS 'Admin'@'%' IDENTIFIED BY 'h7G7I4&qI';
CREATE USER IF NOT EXISTS 'Usuario'@'10.120.2.%' IDENTIFIED BY 'B8d1(3@RU';
CREATE USER IF NOT EXISTS 'UsuarioLocal'@'localhost' IDENTIFIED BY 'J2Qz29+Tj';

-- Asignación de permisos para Admin
GRANT ALL PRIVILEGES ON 5to_Spotify.* TO 'Admin'@'localhost';
GRANT ALL PRIVILEGES ON 5to_Spotify.* TO 'Admin'@'%';

-- Asignación de permisos para Usuario
GRANT SELECT, INSERT, UPDATE ON 5to_Spotify.Usuario TO 'Usuario'@'10.120.2.%';
GRANT SELECT ON 5to_Spotify.Cancion TO 'Usuario'@'10.120.2.%';

--  de permisos para UsuarioLocal
GRANT SELECT ON 5to_Spotify.* TO 'UsuarioLocal'@'localhost';

-- Creación de un nuevo usuario con permisos específicos
CREATE USER 'Gestor'@'localhost' IDENTIFIED BY 'G3st0rP@ss';
GRANT SELECT, INSERT, UPDATE, DELETE ON 5to_Spotify.Playlist TO 'Gestor'@'localhost';
GRANT SELECT ON 5to_Spotify.Album TO 'Gestor'@'localhost';

-- Aplicar los cambios
FLUSH PRIVILEGES;
Asignación