USE 5to_Spotify;

DELIMITER $$
-- 1. Función para verificar suscripción activa
DROP FUNCTION IF EXISTS EstaSuscrito $$
CREATE FUNCTION EstaSuscrito(unidUsuario INT UNSIGNED)
RETURNS BOOL READS SQL DATA
BEGIN
    DECLARE Suscrito BOOL DEFAULT FALSE;
    
    SELECT EXISTS(
        SELECT 1 
        FROM Suscripcion S
        JOIN TipoSuscripcion TS ON S.idTipoSuscripcion = TS.idTipoSuscripcion
        WHERE S.idUsuario = unidUsuario
        AND DATE_ADD(S.FechaInicio, INTERVAL TS.Duracion MONTH) >= CURDATE()
    ) INTO Suscrito;
    
    RETURN Suscrito;
END $$
DELIMITER ;

DELIMITER $$
-- 2. Función para obtener nacionalidad del artista
DROP FUNCTION IF EXISTS ObtenerNacionalidadArtista $$
CREATE FUNCTION ObtenerNacionalidadArtista(unidArtista INT UNSIGNED)
RETURNS VARCHAR(45) READS SQL DATA
BEGIN 
    DECLARE artistaNacionalidad VARCHAR(45);
    
    SELECT N.Pais INTO artistaNacionalidad
    FROM Artista A
    JOIN Usuario U ON A.idUsuario = U.idUsuario
    JOIN Nacionalidad N ON U.idNacionalidad = N.idNacionalidad
    WHERE A.idArtista = unidArtista;
    
    RETURN COALESCE(artistaNacionalidad, 'Desconocida');
END $$
DELIMITER ;

DELIMITER $$
-- 3. Función para obtener artista del álbum
DROP FUNCTION IF EXISTS ObtenerArtistaAlbum $$
CREATE FUNCTION ObtenerArtistaAlbum(unidAlbum INT UNSIGNED)
RETURNS VARCHAR(35) READS SQL DATA
BEGIN 
    DECLARE nombreArtista VARCHAR(35);
    
    SELECT A.NombreArtistico INTO nombreArtista
    FROM Artista A
    JOIN Album AL ON A.idArtista = AL.idArtista
    WHERE AL.idAlbum = unidAlbum;
    
    RETURN nombreArtista;
END $$
DELIMITER

DELIMITER $$
-- 4. Función para obtener última suscripción
DROP FUNCTION IF EXISTS ObtenerUltimaSuscripcion $$
CREATE FUNCTION ObtenerUltimaSuscripcion(unidUsuario INT UNSIGNED)
RETURNS DATE READS SQL DATA
BEGIN 
    DECLARE ultimaFecha DATE;
    
    SELECT MAX(DATE_ADD(S.FechaInicio, INTERVAL TS.Duracion MONTH)) INTO ultimaFecha
    FROM Suscripcion S
    JOIN TipoSuscripcion TS ON S.idTipoSuscripcion = TS.idTipoSuscripcion
    WHERE S.idUsuario = unidUsuario;
    
    RETURN ultimaFecha;
END $$
DELIMITER ;