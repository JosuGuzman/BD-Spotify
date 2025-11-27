USE 5to_Spotify;

-- -----------------------------------------------------
-- Stored Procedure de Altas de las Tablas
-- -----------------------------------------------------

-- 1 AltaArtista
DELIMITER $$

DROP PROCEDURE IF EXISTS altaArtista $$
CREATE PROCEDURE altaArtista (
    IN unNombreArtistico VARCHAR(35), 
    IN unNombreReal VARCHAR(45), 
    IN unApellidoReal VARCHAR(45), 
    OUT unidArtista INT UNSIGNED
)
BEGIN
    DECLARE existe INT;

    IF unNombreArtistico IS NULL OR TRIM(unNombreArtistico) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El Nombre Artístico es obligatorio.';
    END IF;

    SELECT idArtista INTO existe
    FROM Artista
    WHERE NombreArtistico = unNombreArtistico LIMIT 1;

    IF existe IS NOT NULL THEN
        SET unidArtista = existe;
    ELSE
        INSERT INTO Artista (NombreArtistico, NombreReal, ApellidoReal)
        VALUES (unNombreArtistico, unNombreReal, unApellidoReal);
        SET unidArtista = LAST_INSERT_ID();
    END IF;
END $$

DELIMITER;

-- 2 altaAlbum
DELIMITER $$

DROP PROCEDURE IF EXISTS altaAlbum $$
CREATE PROCEDURE altaAlbum (
    OUT unidAlbum INT UNSIGNED,
    IN unTitulo VARCHAR(45),
    IN unidArtista INT UNSIGNED,
    IN unPortada VARCHAR(255)
)
BEGIN
    IF unTitulo IS NULL OR TRIM(unTitulo) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El título del álbum es obligatorio.';
    END IF;

    IF unPortada IS NULL OR unPortada = '' THEN
        SET unPortada = 'default_album.png';
    END IF;

    INSERT INTO Album (Titulo, fechaLanzamiento, idArtista, Portada)
    VALUES(unTitulo, CURDATE(), unidArtista, unPortada);

    SET unidAlbum = LAST_INSERT_ID();
END $$

DELIMITER;

-- 3 altaNacionalidad
DELIMITER $$

DROP PROCEDURE IF EXISTS altaNacionalidad $$
CREATE PROCEDURE altaNacionalidad (
    IN unPais VARCHAR(45), 
    OUT unidNacionalidad INT UNSIGNED
)
BEGIN
    IF unPais IS NULL OR TRIM(unPais) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El nombre del país es obligatorio.';
    END IF;

    INSERT INTO Nacionalidad (Pais)
    VALUES(unPais);
    
    SET unidNacionalidad = LAST_INSERT_ID();
END $$

DELIMITER;

-- 4 altaUsuario
DELIMITER $$

DROP PROCEDURE IF EXISTS altaUsuario $$
CREATE PROCEDURE altaUsuario (
    IN unNombreUsuario VARCHAR(45), 
    IN unEmail VARCHAR(45), 
    IN unaContrasenia VARCHAR(64), 
    IN unidNacionalidad INT UNSIGNED, 
    OUT unidUsuario INT UNSIGNED
)
BEGIN
    IF unEmail IS NULL OR TRIM(unEmail) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El email es obligatorio.';
    END IF;

    IF unNombreUsuario IS NULL OR TRIM(unNombreUsuario) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El nombre de usuario es obligatorio.';
    END IF;

    INSERT INTO Usuario (NombreUsuario, Email, Contrasenia, idNacionalidad)
    VALUES(unNombreUsuario, unEmail, SHA2(unaContrasenia, 256), unidNacionalidad);
    
    SET unidUsuario = LAST_INSERT_ID();
END $$

DELIMITER;

-- 5 altaGenero
DELIMITER $$

DROP PROCEDURE IF EXISTS altaGenero $$
CREATE PROCEDURE altaGenero (
    IN unGenero VARCHAR(45),
    IN unDescripcion TEXT,
    OUT unidGenero TINYINT UNSIGNED
)
BEGIN
    IF unGenero IS NULL OR TRIM(unGenero) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El nombre del género es obligatorio.';
    END IF;
    
    INSERT INTO Genero (Genero, Descripcion)
    VALUES(unGenero, unDescripcion);
    
    SET unidGenero = LAST_INSERT_ID();
END $$

DELIMITER;

-- 6 altaCancion
DELIMITER $$

DROP PROCEDURE IF EXISTS altaCancion $$
CREATE PROCEDURE altaCancion (
    OUT unidCancion INT UNSIGNED, 
    IN unTitulo VARCHAR(45), 
    IN unDuration TIME, 
    IN unidAlbum INT UNSIGNED, 
    IN unidArtista INT UNSIGNED, 
    IN unidGenero TINYINT UNSIGNED,
    IN unArchivoMP3 VARCHAR(255)
)
BEGIN
    IF unTitulo IS NULL OR TRIM(unTitulo) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El título de la canción es obligatorio.';
    END IF;

    INSERT INTO Cancion(Titulo, Duracion, idAlbum, idArtista, idGenero, ArchivoMP3)
    VALUES(unTitulo, unDuration, unidAlbum, unidArtista, unidGenero, unArchivoMP3);
    
    SET unidCancion = LAST_INSERT_ID();
END $$

DELIMITER;

-- 7 altaHistorial_reproduccion
DELIMITER $$

DROP PROCEDURE IF EXISTS altaHistorial_reproduccion $$
CREATE PROCEDURE altaHistorial_reproduccion (
    OUT unidHistorial INT UNSIGNED, 
    IN unidUsuario INT UNSIGNED, 
    IN unidCancion INT UNSIGNED, 
    IN unFechaReproduccion DATETIME
)
BEGIN 
    INSERT INTO HistorialReproduccion (idUsuario, idCancion, FechaReproduccion)
    VALUES(unidUsuario, unidCancion, COALESCE(unFechaReproduccion, NOW()));
    
    SET unidHistorial = LAST_INSERT_ID();
END $$

DELIMITER;

-- 8 altaPlaylist
DELIMITER $$

DROP PROCEDURE IF EXISTS altaPlaylist $$
CREATE PROCEDURE altaPlaylist (
    IN unNombre VARCHAR(20), 
    IN unidUsuario INT UNSIGNED, 
    OUT unidPlaylist INT UNSIGNED
)
BEGIN
    IF unNombre IS NULL OR TRIM(unNombre) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El nombre de la playlist es obligatorio.';
    END IF;

    INSERT INTO Playlist(Nombre, idUsuario)
    VALUES(unNombre, unidUsuario);
    
    SET unidPlaylist = LAST_INSERT_ID();
END $$

DELIMITER;

-- 9 altaTipoSuscripcion
DELIMITER $$

DROP PROCEDURE IF EXISTS altaTipoSuscripcion $$
CREATE PROCEDURE altaTipoSuscripcion (
    OUT unidTipoSuscripcion INT UNSIGNED, 
    IN unaDuracion TINYINT UNSIGNED, 
    IN unCosto TINYINT UNSIGNED, 
    IN unTipoSuscripcion VARCHAR(45)
)
BEGIN
    INSERT INTO TipoSuscripcion (Duracion, Costo, Tipo)
    VALUES(unaDuracion, unCosto, unTipoSuscripcion);
    
    SET unidTipoSuscripcion = LAST_INSERT_ID();
END $$

DELIMITER;

-- 10 altaRegistroSuscripcion
DELIMITER $$

DROP PROCEDURE IF EXISTS altaRegistroSuscripcion $$
CREATE PROCEDURE altaRegistroSuscripcion (
    OUT unidSuscripcion INT UNSIGNED,
    IN unIdUsuario INT UNSIGNED,
    IN unidTipoSuscripcion INT UNSIGNED
)
BEGIN
    INSERT INTO Suscripcion (idUsuario, idTipoSuscripcion, FechaInicio)
    VALUES (unIdUsuario, unidTipoSuscripcion, CURDATE());

    SET unidSuscripcion = LAST_INSERT_ID();
END $$

DELIMITER;

-- 11 altaPlaylistCancion
DELIMITER $$

DROP PROCEDURE IF EXISTS altaPlaylistCancion $$
CREATE PROCEDURE altaPlaylistCancion (
    IN unidCancion INT UNSIGNED,
    IN unidPlaylist INT UNSIGNED
)
BEGIN 
    INSERT INTO Cancion_Playlist(idCancion, idPlaylist)
    VALUES (unidCancion, unidPlaylist);
END $$

DELIMITER;

-- 12 MatcheoCancion
DELIMITER $$

DROP PROCEDURE IF EXISTS MatcheoCancion $$
CREATE PROCEDURE MatcheoCancion(IN InputCancion VARCHAR(45))
BEGIN
    SELECT idCancion, Titulo
    FROM Cancion
    WHERE MATCH(Titulo) AGAINST(CONCAT(InputCancion, '*') IN BOOLEAN MODE);
END $$

DELIMITER ;

-- -----------------------------------------------------
-- Stored Procedure de Eliminaciones de las Tablas
-- -----------------------------------------------------

-- eliminarArtista

DELIMITER $$

DROP PROCEDURE IF EXISTS eliminarArtista $$
CREATE PROCEDURE eliminarArtista(IN unidArtista INT UNSIGNED)
BEGIN
    -- Eliminar historial de reproducción de canciones del artista
    DELETE HR
    FROM HistorialReproduccion HR
    INNER JOIN Cancion C ON HR.idCancion = C.idCancion
    WHERE C.idArtista = unidArtista;

    -- Eliminar likes
    DELETE MG
    FROM MeGusta MG
    INNER JOIN Cancion C ON MG.idCancion = C.idCancion
    WHERE C.idArtista = unidArtista;

    -- Eliminar canciones del artista
    DELETE FROM Cancion WHERE idArtista = unidArtista;

    -- Eliminar álbumes del artista
    DELETE FROM Album WHERE idArtista = unidArtista;

    -- Finalmente eliminar artista
    DELETE FROM Artista WHERE idArtista = unidArtista;

    SELECT ROW_COUNT() AS FilasEliminadas;
END $$

DELIMITER ;

-- eliminarAlbum

DELIMITER $$

DROP PROCEDURE IF EXISTS eliminarAlbum $$
CREATE PROCEDURE eliminarAlbum(IN unidAlbum INT UNSIGNED)
BEGIN
    -- Eliminar historial y likes de todas las canciones del álbum
    DELETE HR
    FROM HistorialReproduccion HR
    INNER JOIN Cancion C ON HR.idCancion = C.idCancion
    WHERE C.idAlbum = unidAlbum;

    DELETE MG
    FROM MeGusta MG
    INNER JOIN Cancion C ON MG.idCancion = C.idCancion
    WHERE C.idAlbum = unidAlbum;

    -- Eliminar canciones del álbum
    DELETE FROM Cancion WHERE idAlbum = unidAlbum;

    -- Eliminar álbum
    DELETE FROM Album WHERE idAlbum = unidAlbum;

    SELECT ROW_COUNT() AS FilasEliminadas;
END $$

DELIMITER ;

-- eliminarCancion

DELIMITER $$

DROP PROCEDURE IF EXISTS eliminarCancion $$
CREATE PROCEDURE eliminarCancion(IN unidCancion INT UNSIGNED)
BEGIN
    DELETE FROM HistorialReproduccion WHERE idCancion = unidCancion;
    DELETE FROM MeGusta WHERE idCancion = unidCancion;
    DELETE FROM Cancion_Playlist WHERE idCancion = unidCancion;
    DELETE FROM Cancion WHERE idCancion = unidCancion;

    SELECT ROW_COUNT() AS FilasEliminadas;
END $$

DELIMITER ;

-- eliminarUsuario

DELIMITER $$

DROP PROCEDURE IF EXISTS eliminarUsuario $$
CREATE PROCEDURE eliminarUsuario(IN unidUsuario INT UNSIGNED)
BEGIN
    DELETE FROM HistorialReproduccion WHERE idUsuario = unidUsuario;
    DELETE FROM MeGusta WHERE idUsuario = unidUsuario;
    DELETE FROM Suscripcion WHERE idUsuario = unidUsuario;
    DELETE FROM Playlist WHERE idUsuario = unidUsuario;
    DELETE FROM Usuario WHERE idUsuario = unidUsuario;

    SELECT ROW_COUNT() AS FilasEliminadas;
END $$

DELIMITER ;

-- -----------------------------------------------------
-- Stored Procedure de Búsquedas de las Tablas
-- -----------------------------------------------------
DELIMITER $$

CREATE PROCEDURE `BuscarContenido`(
    IN pTerminoBusqueda VARCHAR(100),
    IN pTipoBusqueda ENUM('canciones', 'artistas', 'albumes', 'playlists')
)
BEGIN
    IF pTipoBusqueda = 'canciones' THEN
        SELECT c.idCancion, c.Titulo, a.NombreArtistico, al.Titulo AS Album
        FROM Cancion c
        JOIN Artista a ON c.idArtista = a.idArtista
        JOIN Album al ON c.idAlbum = al.idAlbum
        WHERE MATCH(c.Titulo) AGAINST (pTerminoBusqueda IN NATURAL LANGUAGE MODE)
        AND c.EstaActiva = TRUE
        LIMIT 50;

    ELSEIF pTipoBusqueda = 'artistas' THEN
        SELECT idArtista, NombreArtistico, FotoArtista
        FROM Artista
        WHERE MATCH(NombreArtistico) AGAINST (pTerminoBusqueda IN NATURAL LANGUAGE MODE)
        AND EstaActivo = TRUE
        LIMIT 50;

    ELSEIF pTipoBusqueda = 'albumes' THEN
        SELECT idAlbum, Titulo, idArtista
        FROM Album
        WHERE MATCH(Titulo) AGAINST (pTerminoBusqueda IN NATURAL LANGUAGE MODE)
        LIMIT 50;

    ELSEIF pTipoBusqueda = 'playlists' THEN
        SELECT idPlaylist, Nombre, idUsuario
        FROM Playlist
        WHERE Nombre LIKE CONCAT('%', pTerminoBusqueda, '%')
        LIMIT 50;
    END IF;
END $$

DELIMITER ;

-- -----------------------------------------------------
-- Stored Procedure de Registro de Usuarios Nuevos
-- -----------------------------------------------------
DELIMITER $$

CREATE PROCEDURE `RegistrarUsuario`(
    IN pNombreUsuario VARCHAR(45),
    IN pEmail VARCHAR(45),
    IN pContrasenia VARCHAR(64),
    IN pIdNacionalidad INT UNSIGNED,
    OUT pIdUsuario INT UNSIGNED
)
BEGIN
    START TRANSACTION;
    
    -- Insertar usuario con hash SHA2
    INSERT INTO Usuario(NombreUsuario, Email, Contrasenia, idNacionalidad, idRol)
    VALUES(pNombreUsuario, pEmail, SHA2(pContrasenia, 256), pIdNacionalidad, 2);
    
    SET pIdUsuario = LAST_INSERT_ID();
    
    -- Crear playlist "Me Gusta" por defecto
    INSERT INTO Playlist(Nombre, idUsuario, EsPublica)
    VALUES('Mis Me Gusta', pIdUsuario, FALSE);
    
    COMMIT;
END $$

DELIMITER ;

-- -----------------------------------------------------
-- Stored Procedure de Obtención de Datos de las Tablas
-- -----------------------------------------------------
DELIMITER $$

CREATE PROCEDURE `ObtenerEstadisticasSistema`()
BEGIN
    -- Total de usuarios activos
    SELECT COUNT(*) AS TotalUsuarios
    FROM Usuario
    WHERE EstaActivo = TRUE;

    -- Total de canciones activas
    SELECT COUNT(*) AS TotalCanciones
    FROM Cancion
    WHERE EstaActiva = TRUE;

    -- Total de artistas activos
    SELECT COUNT(*) AS TotalArtistas
    FROM Artista
    WHERE EstaActivo = TRUE;

    -- Reproducciones del día (usando rango de fecha para índice)
    SELECT COUNT(*) AS ReproduccionesHoy
    FROM HistorialReproduccion
    WHERE FechaReproduccion >= CURDATE()
      AND FechaReproduccion < CURDATE() + INTERVAL 1 DAY;
END $$

DELIMITER ;

-- -----------------------------------------------------
-- Stored Procedure de Obterner Tablas
-- -----------------------------------------------------
USE 5to_Spotify;

-- Obtener Artistas
DROP PROCEDURE IF EXISTS ObtenerArtistas $$
CREATE PROCEDURE ObtenerArtistas(IN pLimit INT)
BEGIN
    SELECT idArtista, NombreArtistico, NombreReal, ApellidoReal, FotoArtista, EstaActivo
    FROM Artista
    WHERE EstaActivo = TRUE
    ORDER BY NombreArtistico ASC
    LIMIT pLimit;
END $$

-- Obtener Canciones
DROP PROCEDURE IF EXISTS ObtenerCanciones $$
CREATE PROCEDURE ObtenerCanciones(IN pLimit INT)
BEGIN
    SELECT idCancion, Titulo, Duracion, idAlbum, idArtista, idGenero
    FROM Cancion
    WHERE EstaActiva = TRUE
    ORDER BY Titulo ASC
    LIMIT pLimit;
END $$

-- Obtener Albumes
DROP PROCEDURE IF EXISTS ObtenerAlbum $$
CREATE PROCEDURE ObtenerAlbum(IN pLimit INT)
BEGIN
    SELECT idAlbum, Titulo, fechaLanzamiento, idArtista
    FROM Album
    ORDER BY Titulo ASC
    LIMIT pLimit;
END $$

-- Obtener Generos
DROP PROCEDURE IF EXISTS ObtenerGeneros $$
CREATE PROCEDURE ObtenerGeneros(IN pLimit INT)
BEGIN
    SELECT idGenero, Genero, Descripcion
    FROM Genero
    ORDER BY Genero ASC
    LIMIT pLimit;
END $$

-- Obtener Nacionalidades
DROP PROCEDURE IF EXISTS ObtenerNacionalidades $$
CREATE PROCEDURE ObtenerNacionalidades(IN pLimit INT)
BEGIN
    SELECT idNacionalidad, Pais
    FROM Nacionalidad
    ORDER BY Pais ASC
    LIMIT pLimit;
END $$

-- Obtener PlayLists
DROP PROCEDURE IF EXISTS ObtenerPlayLists $$
CREATE PROCEDURE ObtenerPlayLists(IN pLimit INT)
BEGIN
    SELECT idPlaylist, Nombre, idUsuario, EsPublica
    FROM Playlist
    ORDER BY Nombre ASC
    LIMIT pLimit;
END $$

-- Obtener HistorialReproduccion
DROP PROCEDURE IF EXISTS ObtenerHistorialReproduccion $$
CREATE PROCEDURE ObtenerHistorialReproduccion(IN pLimit INT)
BEGIN
    SELECT idHistorial, idUsuario, idCancion, FechaReproduccion
    FROM HistorialReproduccion
    ORDER BY FechaReproduccion DESC
    LIMIT pLimit;
END $$

-- Obtener Suscripciones
DROP PROCEDURE IF EXISTS ObtenerSuscripciones $$
CREATE PROCEDURE ObtenerSuscripciones(IN pLimit INT)
BEGIN
    SELECT idSuscripcion, idUsuario, idTipoSuscripcion, FechaInicio
    FROM Suscripcion
    ORDER BY FechaInicio DESC
    LIMIT pLimit;
END $$

-- Obtener TipoSuscripciones
DROP PROCEDURE IF EXISTS ObtenerTipoSuscripciones $$
CREATE PROCEDURE ObtenerTipoSuscripciones(IN pLimit INT)
BEGIN
    SELECT idTipoSuscripcion, Duracion, Costo, Tipo
    FROM TipoSuscripcion
    ORDER BY Tipo ASC
    LIMIT pLimit;
END $$

-- Obtener Usuarios
DELIMITER $$

DROP PROCEDURE IF EXISTS ObtenerUsuarios $$
CREATE PROCEDURE ObtenerUsuarios(IN pLimit INT)
BEGIN
    SELECT idUsuario, NombreUsuario, Email, idNacionalidad, idRol, FechaRegistro
    FROM Usuario
    WHERE EstaActivo = TRUE
    ORDER BY NombreUsuario ASC
    LIMIT pLimit;
END $$

-- Buscar Canciones por Titulo
DROP PROCEDURE IF EXISTS BuscarCancionesPorTitulo $$
CREATE PROCEDURE BuscarCancionesPorTitulo(
    IN unTitulo VARCHAR(100),
    IN pLimit INT
)
BEGIN
    SELECT idCancion, Titulo, Duracion, idAlbum, idArtista, idGenero
    FROM Cancion
    WHERE EstaActiva = TRUE
      AND MATCH(Titulo) AGAINST(unTitulo IN NATURAL LANGUAGE MODE)
    LIMIT pLimit;
END $$

DELIMITER ;

-- -----------------------------------------------------
-- Stored Procedure Adicionales
-- -----------------------------------------------------
