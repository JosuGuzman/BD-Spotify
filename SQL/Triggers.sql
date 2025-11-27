USE 5to_Spotify;

DELIMITER $$

-- 1. Trigger para validar canción antes de insertar
DROP TRIGGER IF EXISTS ValidarInsercionCancion $$
CREATE TRIGGER ValidarInsercionCancion BEFORE INSERT ON Cancion
FOR EACH ROW 
BEGIN 
    IF (NEW.Titulo IS NULL OR TRIM(NEW.Titulo) = '') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El título de la canción no puede estar vacío';
    END IF;
END $$

-- 2. Trigger para crear playlist "Me Gusta" automáticamente
DROP TRIGGER IF EXISTS CrearPlaylistMeGusta $$
CREATE TRIGGER CrearPlaylistMeGusta AFTER INSERT ON Usuario
FOR EACH ROW
BEGIN
    INSERT INTO Playlist (Nombre, idUsuario, EsPublica)
    VALUES ('Mis Me Gusta', NEW.idUsuario, FALSE);
END $$

-- 3. Trigger para validar actualización de playlist
DROP TRIGGER IF EXISTS ValidarActualizacionPlaylist $$
CREATE TRIGGER ValidarActualizacionPlaylist BEFORE UPDATE ON Playlist
FOR EACH ROW
BEGIN
    IF (NEW.Nombre IS NULL OR TRIM(NEW.Nombre) = '') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El nombre de la playlist no puede estar vacío';
    END IF;
END $$

-- 4. Trigger para limpiar canciones de playlist eliminada
DROP TRIGGER IF EXISTS LimpiarPlaylistEliminada $$
CREATE TRIGGER LimpiarPlaylistEliminada AFTER DELETE ON Playlist
FOR EACH ROW
BEGIN
    DELETE FROM Cancion_Playlist
    WHERE idPlaylist = OLD.idPlaylist;
END $$

DELIMITER ;