USE 5to_Spotify;

-- 1
DELIMITER $$
DROP FUNCTION IF EXISTS estaSuscrito $$
CREATE FUNCTION estaSuscrito (unidUsuario INT)
RETURNS BOOL READS SQL DATA
BEGIN
	DECLARE Suscrito BOOL;
    IF (EXISTS(SELECT * 
			   FROM Usuario_suscripcion US
               WHERE US.idUsuario = unidUsuario
               JOIN Suscripcion S ON US.idSuscripcion = S.idSuscripcion
               AND DATE_ADD(US.FechaInicio, INTERVAL S.Duracion MONTH) >= CURDATE())) THEN
        SET Suscrito = True;
	ELSE
		SET Suscrito = False;
	END IF;
    RETURN Suscrito;
END $$

-- 2
DELIMITER $$
DROP FUNCTION IF EXISTS nacionalidadArtista $$
CREATE FUNCTION nacionalidadArtista (unidArtista INT)
RETURNS VARCHAR(45) READS SQL DATA
BEGIN 
	DECLARE artistaNacionalidad VARCHAR(45);
	
    SELECT Pais into artistaNacionalidad
    FROM Artista
    JOIN Nacionalidad USING (idNacionalidad)
    WHERE idArtista = unidArtista
    AND Nacionalidad.idNacionalidad = Artista.Nacionalidad;
	
    RETURN artistaNacionalidad;
END $$

-- 3
DELIMITER $$
DROP FUNCTION IF EXISTS artistaAlbum $$
CREATE FUNCTION artistaAlbum (unidAlbum INT)
RETURNS VARCHAR(35) READS SQL DATA
BEGIN 
	DECLARE artista VARCHAR(35);
    
    SELECT A.NombreArtistico into artista
    FROM Artista A
    JOIN Album USING (idArtista)
    WHERE A.idArtista = Album.idArtista
    AND idAlbum = unidAlbum;
    
    RETURN artista;
END $$

-- 3
DELIMITER $$
DROP FUNCTION IF EXISTS ultimaSuscripcion $$
CREATE FUNCTION ultimaSuscripcion (unidUsuario INT)
RETURNS DATE READS SQL DATA
BEGIN 
    DECLARE ultimoDia DATE;

    SELECT MAX(DATE_ADD(US.FechaInicio, INTERVAL S.Duracion MONTH)) INTO ultimoDia
    FROM Usuario_suscripcion US
    JOIN Suscripcion S ON US.idSuscripcion = S.idSuscripcion
    WHERE US.idUsuario = unidUsuario;

    RETURN ultimoDia;
END $$

-- 4
