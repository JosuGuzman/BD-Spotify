USE 5to_Spotify;

-- 1
DELIMITER $$
DROP TRIGGER IF EXISTS befInsertCancion $$
CREATE TRIGGER befInsertCancion BEFORE INSERT ON Cancion
FOR EACH ROW 
BEGIN 
	IF(NEW.Titulo = '') THEN
    SIGNAL SQLSTATE '45000'
    SET MESSAGE_TEXT = 'El titulo no puede estar vacio';
	END IF;
END $$

-- 2
DELIMITER $$
DROP TRIGGER IF EXISTS aftInsertUsuario $$
CREATE TRIGGER aftInsertUsuario AFTER INSERT ON Usuario
FOR EACH ROW
BEGIN
	INSERT INTO Playlist (Nombre,idUsuario)
		VALUES('Tus Megusta',NEW.idUsuario);
END $$

-- 3
DELIMITER $$
DROP TRIGGER IF EXISTS befUpdatePlaylist$$
CREATE TRIGGER befUpdatePlaylist BEFORE UPDATE ON Playlist
FOR EACH ROW
BEGIN
	IF(NEW.Nombre = '')THEN
	SIGNAL SQLSTATE '45000'
    SET MESSAGE_TEXT = 'El nombre no puede estar vacio';
	END IF;
END $$

-- 4
DELIMITER $$
DROP TRIGGER IF EXISTS aftDeletePlaylist $$
CREATE TRIGGER aftDeletePlaylist AFTER DELETE ON Playlist
FOR EACH ROW
BEGIN
	DELETE FROM Playlist_Cancion
    WHERE idPlaylist = OLD.idPlaylist;
END $$