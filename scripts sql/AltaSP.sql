USE 5to_Spotify;

-- Tabla Artista
DELIMITER $$
DROP PROCEDURE IF EXISTS altaArtista $$
CREATE PROCEDURE altaArtista (
    unNombreArtistico VARCHAR(35), 
    unNombre VARCHAR(45), 
    unApellido VARCHAR(45), 
    OUT unidArtista INT UNSIGNED
)
BEGIN 
    INSERT INTO Artista(NombreArtistico, Nombre, Apellido)
    VALUES(unNombreArtistico, unNombre, unApellido);
    
    SET unidArtista = LAST_INSERT_ID();
END $$

-- Tabla Album (MODIFICADO - incluye portada)
DELIMITER $$
DROP PROCEDURE IF EXISTS altaAlbum $$
CREATE PROCEDURE altaAlbum (
    OUT unidAlbum INT UNSIGNED,
    unTitulo VARCHAR(45),
    unidArtista INT UNSIGNED,
    unPortada VARCHAR(255)
)
BEGIN 
    -- Si no se proporciona portada, usar la predeterminada
    IF unPortada IS NULL OR unPortada = '' THEN
        SET unPortada = 'default_album.png';
    END IF;
    
    INSERT INTO Album (Titulo, fechaLanzamiento, idArtista, Portada)
    VALUES(unTitulo, CURDATE(), unidArtista, unPortada);

    SET unidAlbum = LAST_INSERT_ID();
END $$

-- Tabla Nacionalidad
DELIMITER $$
DROP PROCEDURE IF EXISTS altaNacionalidad $$
CREATE PROCEDURE altaNacionalidad (
    unPais VARCHAR(45), 
    OUT unidNacionalidad INT UNSIGNED
)
BEGIN
    INSERT INTO Nacionalidad (Pais)
    VALUES(unPais);
    
    SET unidNacionalidad = LAST_INSERT_ID();
END $$

-- Tabla Usuario
DELIMITER $$
DROP PROCEDURE IF EXISTS altaUsuario $$
CREATE PROCEDURE altaUsuario (
    unNombreUsuario VARCHAR(45), 
    unEmail VARCHAR(45), 
    unaContrasenia VARCHAR(64), 
    unidNacionalidad INT UNSIGNED, 
    OUT unidUsuario INT UNSIGNED
)
BEGIN
    INSERT INTO Usuario(NombreUsuario, Email, Contrasenia, idNacionalidad)
    VALUES(unNombreUsuario, unEmail, SHA2(unaContrasenia, 256), unidNacionalidad);
    
    SET unidUsuario = LAST_INSERT_ID();
END $$

-- Tabla Genero (MODIFICADO - incluye descripción)
DELIMITER $$
DROP PROCEDURE IF EXISTS altaGenero $$
CREATE PROCEDURE altaGenero (
    unGenero VARCHAR(45),
    unDescripcion TEXT,
    OUT unidGenero TINYINT UNSIGNED
)
BEGIN
    INSERT INTO Genero (Genero, Descripcion)
    VALUES(unGenero, unDescripcion);
    
    SET unidGenero = LAST_INSERT_ID();
END $$

-- Tabla Cancion (MODIFICADO - incluye archivo MP3)
DELIMITER $$
DROP PROCEDURE IF EXISTS altaCancion $$
CREATE PROCEDURE altaCancion (
    OUT unidCancion INT UNSIGNED, 
    unTitulo VARCHAR(45), 
    unDuration TIME, 
    unidAlbum INT UNSIGNED, 
    unidArtista INT UNSIGNED, 
    unidGenero TINYINT UNSIGNED,
    unArchivoMP3 VARCHAR(255)
)
BEGIN 
    INSERT INTO Cancion(Titulo, Duracion, idAlbum, idArtista, idGenero, ArchivoMP3)
    VALUES(unTitulo, unDuration, unidAlbum, unidArtista, unidGenero, unArchivoMP3);
    
    SET unidCancion = LAST_INSERT_ID();
END $$

-- Tabla Historial Reproduccion
DELIMITER $$
DROP PROCEDURE IF EXISTS altaHistorial_reproduccion $$
CREATE PROCEDURE altaHistorial_reproduccion (
    OUT unidHistorial INT UNSIGNED, 
    unidUsuario INT UNSIGNED, 
    unidCancion INT UNSIGNED, 
    unFechaReproduccion DATETIME
)
BEGIN 
    INSERT INTO HistorialReproduccion (idUsuario, idCancion, FechaReproduccion)
    VALUES(unidUsuario, unidCancion, unFechaReproduccion);
    
    SET unidHistorial = LAST_INSERT_ID();
END $$

-- Tabla Playlist
DELIMITER $$
DROP PROCEDURE IF EXISTS altaPlaylist $$
CREATE PROCEDURE altaPlaylist (
    unNombre VARCHAR(20), 
    unidUsuario INT UNSIGNED, 
    OUT unidPlaylist INT UNSIGNED
)
BEGIN
    INSERT INTO Playlist(Nombre, idUsuario)
    VALUES(unNombre, unidUsuario);
    
    SET unidPlaylist = LAST_INSERT_ID();
END $$

-- Tabla TipoSuscripcion
DELIMITER $$
DROP PROCEDURE IF EXISTS altaTipoSuscripcion $$
CREATE PROCEDURE altaTipoSuscripcion (
    OUT unidTipoSuscripcion INT UNSIGNED, 
    unaDuracion TINYINT UNSIGNED, 
    unCosto TINYINT UNSIGNED, 
    UntipoSuscripcion VARCHAR(45)
)
BEGIN
    INSERT INTO TipoSuscripcion (Duracion, Costo, Tipo)
    VALUES(unaDuracion, unCosto, UntipoSuscripcion);
    
    SET unidTipoSuscripcion = LAST_INSERT_ID();
END $$

-- Registro Suscripcion
DELIMITER $$
DROP PROCEDURE IF EXISTS altaRegistroSuscripcion $$
CREATE PROCEDURE altaRegistroSuscripcion (
    OUT unidSuscripcion INT UNSIGNED,
    unIdUsuario INT UNSIGNED,
    unidTipoSuscripcion INT UNSIGNED
)
BEGIN
    INSERT INTO Suscripcion (idUsuario, idTipoSuscripcion, FechaInicio)
    VALUES (unIdUsuario, unidTipoSuscripcion, CURDATE());

    SET unidSuscripcion = LAST_INSERT_ID();
END $$

-- Playlist Cancion
DELIMITER $$
DROP PROCEDURE IF EXISTS altaPlaylistCancion $$
CREATE PROCEDURE altaPlaylistCancion (
    unidCancion INT UNSIGNED,
    unidPlaylist INT UNSIGNED
)
BEGIN 
    INSERT INTO Cancion_Playlist(idCancion, idPlaylist)
    VALUES (unidCancion, unidPlaylist);
END $$

-- Búsqueda de canciones
DELIMITER $$
DROP PROCEDURE IF EXISTS MatcheoCancion $$
CREATE PROCEDURE MatcheoCancion(InputCancion VARCHAR(45))
BEGIN
    SELECT Titulo
    FROM Cancion
    WHERE MATCH(Titulo) AGAINST(CONCAT(InputCancion, "*") IN BOOLEAN MODE);
END $$