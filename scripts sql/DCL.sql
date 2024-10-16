-- Creación de usuarios
CREATE USER 'Admin'@'localhost' IDENTIFIED BY 'adiMn456?';
CREATE USER 'Admin'@'%' IDENTIFIED BY 'Adimer213#';

-- Asignación de permisos para Admin
GRANT ALL PRIVILEGES ON 5to_Spotify.* TO 'Admin'@'localhost';
GRANT ALL PRIVILEGES ON 5to_Spotify.* TO 'Admin'@'%';

-- Asignación de permisos para Manager

-- Aplicar los cambios
FLUSH PRIVILEGES;