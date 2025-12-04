USE 5to_Spotify;

DELIMITER $$

-- 1. Trigger para crear playlist "Me Gusta" automáticamente
DROP TRIGGER IF EXISTS CrearPlaylistMeGusta $$
CREATE TRIGGER CrearPlaylistMeGusta 
AFTER INSERT ON Usuario
FOR EACH ROW
BEGIN
    -- Solo para usuarios registrados (no administradores)
    IF NEW.IdRol = 2 THEN -- 2 = Usuario registrado
        INSERT INTO Playlist (Nombre, IdUsuario, Descripcion, EsPublica, EsSistema)
        VALUES ('Me gusta', NEW.IdUsuario, 'Tus canciones favoritas', FALSE, TRUE);
    END IF;
END $$

-- 2. Trigger para validar inserción de canción
DROP TRIGGER IF EXISTS ValidarInsercionCancion $$
CREATE TRIGGER ValidarInsercionCancion 
BEFORE INSERT ON Cancion
FOR EACH ROW 
BEGIN 
    IF NEW.Titulo IS NULL OR TRIM(NEW.Titulo) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El título de la canción no puede estar vacío';
    END IF;
    
    IF NEW.DuracionSegundos <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La duración debe ser mayor a 0 segundos';
    END IF;
    
    IF NEW.ArchivoMP3 IS NULL OR TRIM(NEW.ArchivoMP3) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El archivo MP3 es obligatorio';
    END IF;
END $$

-- 3. Trigger para validar actualización de playlist
DROP TRIGGER IF EXISTS ValidarActualizacionPlaylist $$
CREATE TRIGGER ValidarActualizacionPlaylist 
BEFORE UPDATE ON Playlist
FOR EACH ROW
BEGIN
    IF NEW.Nombre IS NULL OR TRIM(NEW.Nombre) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El nombre de la playlist no puede estar vacío';
    END IF;
END $$

-- 4. Trigger para limpiar canciones de playlist eliminada
DROP TRIGGER IF EXISTS LimpiarPlaylistEliminada $$
CREATE TRIGGER LimpiarPlaylistEliminada 
AFTER UPDATE ON Playlist
FOR EACH ROW
BEGIN
    -- Si la playlist se desactiva, eliminar sus relaciones
    IF OLD.EstaActiva = 1 AND NEW.EstaActiva = 0 THEN
        DELETE FROM Cancion_Playlist
        WHERE IdPlaylist = NEW.IdPlaylist;
    END IF;
END $$

DELIMITER ;