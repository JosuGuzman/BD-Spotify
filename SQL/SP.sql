USE 5to_Spotify;

-- -----------------------------------------------------
-- Procedimientos Almacenados de Altas
-- -----------------------------------------------------

-- 1. AltaArtista
DELIMITER $$
DROP PROCEDURE IF EXISTS AltaArtista $$
CREATE PROCEDURE AltaArtista(
    IN pNombreArtistico VARCHAR(35),
    IN pNombreReal VARCHAR(45),
    IN pApellidoReal VARCHAR(45),
    IN pIdNacionalidad INT UNSIGNED,
    OUT pIdArtista INT UNSIGNED
)
BEGIN
    DECLARE vExiste INT;

    IF pNombreArtistico IS NULL OR TRIM(pNombreArtistico) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El Nombre Artístico es obligatorio.';
    END IF;

    SELECT IdArtista INTO vExiste
    FROM Artista
    WHERE NombreArtistico = pNombreArtistico 
    AND EstaActivo = 1 
    LIMIT 1;

    IF vExiste IS NOT NULL THEN
        SET pIdArtista = vExiste;
    ELSE
        INSERT INTO Artista (NombreArtistico, NombreReal, ApellidoReal, IdNacionalidad)
        VALUES (pNombreArtistico, pNombreReal, pApellidoReal, pIdNacionalidad);
        SET pIdArtista = LAST_INSERT_ID();
    END IF;
END $$
DELIMITER ;

-- 2. AltaAlbum
DELIMITER $$
DROP PROCEDURE IF EXISTS AltaAlbum $$
CREATE PROCEDURE AltaAlbum(
    IN pTitulo VARCHAR(45),
    IN pIdArtista INT UNSIGNED,
    IN pPortada VARCHAR(255),
    OUT pIdAlbum INT UNSIGNED
)
BEGIN
    IF pTitulo IS NULL OR TRIM(pTitulo) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El título del álbum es obligatorio.';
    END IF;

    IF pPortada IS NULL OR pPortada = '' THEN
        SET pPortada = 'album_default.png';
    END IF;

    INSERT INTO Album (Titulo, FechaLanzamiento, IdArtista, Portada)
    VALUES(pTitulo, CURDATE(), pIdArtista, pPortada);

    SET pIdAlbum = LAST_INSERT_ID();
END $$
DELIMITER ;

-- 3. AltaNacionalidad
DELIMITER $$
DROP PROCEDURE IF EXISTS AltaNacionalidad $$
CREATE PROCEDURE AltaNacionalidad(
    IN pPais VARCHAR(45),
    OUT pIdNacionalidad INT UNSIGNED
)
BEGIN
    IF pPais IS NULL OR TRIM(pPais) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El nombre del país es obligatorio.';
    END IF;

    INSERT INTO Nacionalidad (Pais)
    VALUES(pPais);
    
    SET pIdNacionalidad = LAST_INSERT_ID();
END $$
DELIMITER ;

-- 4. AltaUsuario
DELIMITER $$
DROP PROCEDURE IF EXISTS AltaUsuario $$
CREATE PROCEDURE AltaUsuario(
    IN pNombreUsuario VARCHAR(45),
    IN pEmail VARCHAR(45),
    IN pContrasenia VARCHAR(255),
    IN pIdNacionalidad INT UNSIGNED,
    IN pIdRol TINYINT UNSIGNED,
    OUT pIdUsuario INT UNSIGNED
)
BEGIN
    DECLARE vEmailExistente INT;

    IF pEmail IS NULL OR TRIM(pEmail) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El email es obligatorio.';
    END IF;

    IF pNombreUsuario IS NULL OR TRIM(pNombreUsuario) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El nombre de usuario es obligatorio.';
    END IF;

    -- Verificar si el email ya existe
    SELECT COUNT(*) INTO vEmailExistente 
    FROM Usuario 
    WHERE Email = pEmail 
    AND EstaActivo = 1;

    IF vEmailExistente > 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El email ya está registrado.';
    END IF;

    INSERT INTO Usuario (NombreUsuario, Email, Contrasenia, IdNacionalidad, IdRol)
    VALUES(pNombreUsuario, pEmail, pContrasenia, pIdNacionalidad, pIdRol);
    
    SET pIdUsuario = LAST_INSERT_ID();
END $$
DELIMITER ;

-- 5. AltaGenero
DELIMITER $$
DROP PROCEDURE IF EXISTS AltaGenero $$
CREATE PROCEDURE AltaGenero(
    IN pNombre VARCHAR(45),
    IN pDescripcion TEXT,
    OUT pIdGenero TINYINT UNSIGNED
)
BEGIN
    IF pNombre IS NULL OR TRIM(pNombre) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El nombre del género es obligatorio.';
    END IF;
    
    INSERT INTO Genero (Nombre, Descripcion)
    VALUES(pNombre, pDescripcion);
    
    SET pIdGenero = LAST_INSERT_ID();
END $$
DELIMITER ;

-- 6. AltaCancion
DELIMITER $$
DROP PROCEDURE IF EXISTS AltaCancion $$
CREATE PROCEDURE AltaCancion(
    IN pTitulo VARCHAR(45),
    IN pDuracionSegundos INT UNSIGNED,
    IN pIdAlbum INT UNSIGNED,
    IN pIdArtista INT UNSIGNED,
    IN pIdGenero TINYINT UNSIGNED,
    IN pArchivoMP3 VARCHAR(500),
    OUT pIdCancion INT UNSIGNED
)
BEGIN
    IF pTitulo IS NULL OR TRIM(pTitulo) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El título de la canción es obligatorio.';
    END IF;

    IF pDuracionSegundos <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La duración debe ser mayor a 0 segundos.';
    END IF;

    INSERT INTO Cancion(Titulo, DuracionSegundos, IdAlbum, IdArtista, IdGenero, ArchivoMP3)
    VALUES(pTitulo, pDuracionSegundos, pIdAlbum, pIdArtista, pIdGenero, pArchivoMP3);
    
    SET pIdCancion = LAST_INSERT_ID();
END $$
DELIMITER ;

-- 7. AltaHistorialReproduccion
DELIMITER $$
DROP PROCEDURE IF EXISTS AltaHistorialReproduccion $$
CREATE PROCEDURE AltaHistorialReproduccion(
    IN pIdUsuario INT UNSIGNED,
    IN pIdCancion INT UNSIGNED,
    IN pDuracionReproducida INT UNSIGNED,
    OUT pIdHistorial BIGINT UNSIGNED
)
BEGIN 
    -- Incrementar contador de reproducciones de la canción
    UPDATE Cancion 
    SET ContadorReproducciones = ContadorReproducciones + 1 
    WHERE IdCancion = pIdCancion;
    
    INSERT INTO HistorialReproduccion (IdUsuario, IdCancion, DuracionReproducida)
    VALUES(pIdUsuario, pIdCancion, pDuracionReproducida);
    
    SET pIdHistorial = LAST_INSERT_ID();
END $$
DELIMITER ;

-- 8. AltaPlaylist
DELIMITER $$
DROP PROCEDURE IF EXISTS AltaPlaylist $$
CREATE PROCEDURE AltaPlaylist(
    IN pNombre VARCHAR(100),
    IN pIdUsuario INT UNSIGNED,
    IN pDescripcion TEXT,
    IN pEsPublica BOOLEAN,
    OUT pIdPlaylist INT UNSIGNED
)
BEGIN
    IF pNombre IS NULL OR TRIM(pNombre) = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El nombre de la playlist es obligatorio.';
    END IF;

    INSERT INTO Playlist(Nombre, IdUsuario, Descripcion, EsPublica)
    VALUES(pNombre, pIdUsuario, pDescripcion, pEsPublica);
    
    SET pIdPlaylist = LAST_INSERT_ID();
END $$
DELIMITER ;

-- 9. AltaTipoSuscripcion
DELIMITER $$
DROP PROCEDURE IF EXISTS AltaTipoSuscripcion $$
CREATE PROCEDURE AltaTipoSuscripcion(
    IN pDuracionMeses INT UNSIGNED,
    IN pCosto DECIMAL(10,2),
    IN pTipo VARCHAR(45),
    OUT pIdTipoSuscripcion INT UNSIGNED
)
BEGIN
    INSERT INTO TipoSuscripcion (DuracionMeses, Costo, Tipo)
    VALUES(pDuracionMeses, pCosto, pTipo);
    
    SET pIdTipoSuscripcion = LAST_INSERT_ID();
END $$
DELIMITER ;

-- 10. AltaSuscripcionUsuario
DELIMITER $$
DROP PROCEDURE IF EXISTS AltaSuscripcionUsuario $$
CREATE PROCEDURE AltaSuscripcionUsuario(
    IN pIdUsuario INT UNSIGNED,
    IN pIdTipoSuscripcion INT UNSIGNED,
    OUT pIdSuscripcion INT UNSIGNED
)
BEGIN
    DECLARE vDuracion INT;
    DECLARE vFechaFin DATE;
    
    -- Obtener duración del tipo de suscripción
    SELECT DuracionMeses INTO vDuracion 
    FROM TipoSuscripcion 
    WHERE IdTipoSuscripcion = pIdTipoSuscripcion;
    
    -- Calcular fecha de fin
    SET vFechaFin = DATE_ADD(CURDATE(), INTERVAL vDuracion MONTH);
    
    -- Desactivar suscripciones anteriores
    UPDATE Suscripcion 
    SET Activo = 0 
    WHERE IdUsuario = pIdUsuario AND Activo = 1;
    
    -- Crear nueva suscripción
    INSERT INTO Suscripcion (IdUsuario, IdTipoSuscripcion, FechaInicio, FechaFin, Activo)
    VALUES (pIdUsuario, pIdTipoSuscripcion, CURDATE(), vFechaFin, 1);
    
    SET pIdSuscripcion = LAST_INSERT_ID();
END $$
DELIMITER ;

-- 11. AgregarCancionPlaylist
DELIMITER $$
DROP PROCEDURE IF EXISTS AgregarCancionPlaylist $$
CREATE PROCEDURE AgregarCancionPlaylist(
    IN pIdCancion INT UNSIGNED,
    IN pIdPlaylist INT UNSIGNED
)
BEGIN 
    DECLARE vMaxOrden INT;
    
    -- Obtener el máximo orden actual
    SELECT COALESCE(MAX(Orden), 0) INTO vMaxOrden
    FROM Cancion_Playlist
    WHERE IdPlaylist = pIdPlaylist;
    
    INSERT INTO Cancion_Playlist(IdCancion, IdPlaylist, Orden)
    VALUES (pIdCancion, pIdPlaylist, vMaxOrden + 1);
END $$
DELIMITER ;

-- 12. BuscarCanciones
DELIMITER $$
DROP PROCEDURE IF EXISTS BuscarCanciones $$
CREATE PROCEDURE BuscarCanciones(
    IN pTerminoBusqueda VARCHAR(100),
    IN pIdGenero INT UNSIGNED,
    IN pIdArtista INT UNSIGNED,
    IN pAnio INT UNSIGNED,
    IN pLimite INT
)
BEGIN
    SET @sql = '
        SELECT 
            c.IdCancion, 
            c.Titulo, 
            c.DuracionSegundos,
            a.NombreArtistico AS Artista,
            al.Titulo AS Album,
            g.Nombre AS Genero,
            al.FechaLanzamiento,
            c.ContadorReproducciones
        FROM Cancion c
        JOIN Artista a ON c.IdArtista = a.IdArtista
        JOIN Album al ON c.IdAlbum = al.IdAlbum
        JOIN Genero g ON c.IdGenero = g.IdGenero
        WHERE c.EstaActiva = 1
          AND a.EstaActivo = 1
          AND al.EstaActivo = 1
          AND g.EstaActivo = 1';
    
    IF pTerminoBusqueda IS NOT NULL AND pTerminoBusqueda != '' THEN
        SET @sql = CONCAT(@sql, ' AND MATCH(c.Titulo) AGAINST(? IN BOOLEAN MODE)');
    END IF;
    
    IF pIdGenero IS NOT NULL THEN
        SET @sql = CONCAT(@sql, ' AND c.IdGenero = ?');
    END IF;
    
    IF pIdArtista IS NOT NULL THEN
        SET @sql = CONCAT(@sql, ' AND c.IdArtista = ?');
    END IF;
    
    IF pAnio IS NOT NULL THEN
        SET @sql = CONCAT(@sql, ' AND YEAR(al.FechaLanzamiento) = ?');
    END IF;
    
    SET @sql = CONCAT(@sql, ' ORDER BY c.ContadorReproducciones DESC LIMIT ?');
    
    -- Preparar parámetros
    SET @param1 = pTerminoBusqueda;
    SET @param2 = pIdGenero;
    SET @param3 = pIdArtista;
    SET @param4 = pAnio;
    SET @param5 = pLimite;
    
    PREPARE stmt FROM @sql;
    
    IF pTerminoBusqueda IS NOT NULL AND pIdGenero IS NOT NULL AND pIdArtista IS NOT NULL AND pAnio IS NOT NULL THEN
        EXECUTE stmt USING @param1, @param2, @param3, @param4, @param5;
    ELSEIF pTerminoBusqueda IS NOT NULL AND pIdGenero IS NOT NULL AND pIdArtista IS NOT NULL THEN
        EXECUTE stmt USING @param1, @param2, @param3, @param5;
    ELSEIF pTerminoBusqueda IS NOT NULL AND pIdGenero IS NOT NULL THEN
        EXECUTE stmt USING @param1, @param2, @param5;
    ELSEIF pTerminoBusqueda IS NOT NULL THEN
        EXECUTE stmt USING @param1, @param5;
    ELSEIF pIdGenero IS NOT NULL THEN
        EXECUTE stmt USING @param2, @param5;
    ELSE
        EXECUTE stmt USING @param5;
    END IF;
    
    DEALLOCATE PREPARE stmt;
END $$
DELIMITER ;

-- 13. RegistrarUsuarioCompleto
DELIMITER $$
DROP PROCEDURE IF EXISTS RegistrarUsuarioCompleto $$
CREATE PROCEDURE RegistrarUsuarioCompleto(
    IN pNombreUsuario VARCHAR(45),
    IN pEmail VARCHAR(45),
    IN pContrasenia VARCHAR(255),
    IN pIdNacionalidad INT UNSIGNED,
    OUT pIdUsuario INT UNSIGNED,
    OUT pIdPlaylistMeGusta INT UNSIGNED
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    -- Insertar usuario (rol 2 = Usuario Registrado)
    CALL AltaUsuario(pNombreUsuario, pEmail, pContrasenia, pIdNacionalidad, 2, pIdUsuario);
    
    -- Crear playlist "Me gusta" por defecto (privada)
    CALL AltaPlaylist('Me gusta', pIdUsuario, 'Tus canciones favoritas', FALSE, pIdPlaylistMeGusta);
    
    COMMIT;
END $$
DELIMITER ;

-- 14. ObtenerEstadisticasSistema
DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerEstadisticasSistema $$
CREATE PROCEDURE ObtenerEstadisticasSistema()
BEGIN
    -- Totales generales
    SELECT 
        (SELECT COUNT(*) FROM Usuario WHERE EstaActivo = 1 AND IdRol = 2) AS TotalUsuariosRegistrados,
        (SELECT COUNT(*) FROM Usuario WHERE EstaActivo = 1 AND IdRol = 3) AS TotalAdministradores,
        (SELECT COUNT(*) FROM Usuario WHERE DATE(FechaRegistro) = CURDATE()) AS NuevosUsuariosHoy,
        (SELECT COUNT(*) FROM Cancion WHERE EstaActiva = 1) AS TotalCanciones,
        (SELECT COUNT(*) FROM Artista WHERE EstaActivo = 1) AS TotalArtistas,
        (SELECT COUNT(*) FROM Album WHERE EstaActivo = 1) AS TotalAlbumes,
        (SELECT COUNT(*) FROM Playlist WHERE EstaActiva = 1) AS TotalPlaylists,
        (SELECT SUM(ContadorReproducciones) FROM Cancion) AS TotalReproducciones,
        (SELECT COUNT(*) FROM HistorialReproduccion WHERE DATE(FechaReproduccion) = CURDATE()) AS ReproduccionesHoy,
        (SELECT COUNT(*) FROM Suscripcion WHERE Activo = 1) AS SuscripcionesActivas
    FROM dual;
END $$
DELIMITER ;

-- 15. VerificarCredenciales
DELIMITER $$
DROP PROCEDURE IF EXISTS VerificarCredenciales $$
CREATE PROCEDURE VerificarCredenciales(
    IN pEmail VARCHAR(150),
    IN pContrasenia VARCHAR(255),
    OUT pIdUsuario INT UNSIGNED,
    OUT pNombreUsuario VARCHAR(60),
    OUT pIdRol TINYINT UNSIGNED,
    OUT pFotoPerfil VARCHAR(255),
    OUT pValido BOOLEAN
)
BEGIN
    DECLARE vHashAlmacenado VARCHAR(255);
    
    SELECT 
        IdUsuario, 
        NombreUsuario, 
        IdRol, 
        FotoPerfil,
        Contrasenia INTO pIdUsuario, pNombreUsuario, pIdRol, pFotoPerfil, vHashAlmacenado
    FROM Usuario
    WHERE Email = pEmail AND EstaActivo = 1
    LIMIT 1;
    
    -- Comparar contraseñas (en producción usar bcrypt.compare)
    IF vHashAlmacenado = pContrasenia THEN
        SET pValido = TRUE;
        
        -- Actualizar último acceso
        UPDATE Usuario 
        SET UltimoAcceso = NOW() 
        WHERE IdUsuario = pIdUsuario;
    ELSE
        SET pValido = FALSE;
        SET pIdUsuario = NULL;
        SET pNombreUsuario = NULL;
        SET pIdRol = NULL;
        SET pFotoPerfil = NULL;
    END IF;
END $$
DELIMITER ;