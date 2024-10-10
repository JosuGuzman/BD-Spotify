CREATE USER 'nombre_usuario'@'localhost' IDENTIFIED BY 'contraseña';

GRANT SELECT, INSERT, UPDATE ON 5to_Spotify.* TO 'nombre_usuario'@'localhost';

DROP USER 'nombre_usuario'@'localhost';