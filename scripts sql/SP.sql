USE Spotify;

-- Tabla Artista
DELIMITER $$
DROP PROCEDURE IF EXISTS altaArtista $$
CREATE PROCEDURE altaArtista (unNombreArtistico VARCHAR(35), unNombre VARCHAR(45), unApellido VARCHAR(45), out idArtista INT UNSIGNED)
BEGIN 
    INSERT INTO Artista(NombreArtistico,Nombre,Apellido)
   	    VALUES(unNombreArtistico,unNombre,unApellido);
    
	SET idArtista = last_insert_id();
END $$

-- Tabla Albun
DELIMITER $$
DROP PROCEDURE IF EXISTS altaAlbum $$
CREATE PROCEDURE altaAlbum (OUT unidAlbun INT UNSIGNED,
    unTitulo VARCHAR(45),
    unidArtista INT UNSIGNED)
BEGIN 
	INSERT INTO Album (Titulo,fechaLanzamiento,idArtista)
		VALUES(unTitulo,CURDATE(),unidArtista)

	SET unidAlbun = last_insert_id();
END$$


-- Tabla Nacionalidad
DELIMITER $$
DROP PROCEDURE IF EXISTS altaNacionalidad $$
CREATE PROCEDURE altaNacionalidad (unPais VARCHAR(45), out unidNacionalidad INT UNSIGNED)
BEGIN
    INSERT INTO Nacionalidad (Pais)
        VALUES(unPais);
    
    SET unidNacionalidad = last_insert_id();
END $$

-- Tabla Usuario
DELIMITER $$
DROP PROCEDURE IF EXISTS altaUsuario $$
CREATE PROCEDURE altaUsuario (unNombreUSuario VARCHAR(45), unEmail VARCHAR(45), unaContraseña VARCHAR(64), unidNacionalidad INT UNSIGNED, out unidUsuario INT UNSIGNED)
BEGIN
    INSERT INTO Usuario(NombreUsuario,Email,Contraseña,idNacionalidad)
   	    VALUES(unNombreUsuario,unEmail,unaContraseña,unidNacionalidad);
    
	SET unidUsuario = last_insert_id();
END $$

-- Tabla Genero
DELIMITER $$
DROP PROCEDURE IF EXISTS altaGenero $$
CREATE PROCEDURE altaGenero (unGenero VARCHAR(45), out unidGenero tinyint unsigned)
BEGIN
    INSERT INTO Genero (Genero)
        VALUES(unGenero);
    
	SET unidGenero = last_insert_id();
END$$

-- Tabla Cancion
DELIMITER $$
DROP PROCEDURE IF EXISTS altaCancion $$
CREATE PROCEDURE altaCancion (OUT unidCancion INT UNSGINED, unTitulo VARCHAR(45), unDuration Time, unidAlbun INT UNSIGNED, unidArtista INT UNSIGNED, unidGenero TINYINT UNSIGNED)
BEGIN 
	INSERT INTO Cancion(Titulo,duration,idAlbun,idArtista,idGenero)
		VALUES(unTItulo,unDuration,unidAlbun,unidArtista,unidGenero)
	
	SET unidCancion = last_insert_id();
END $$

-- Tabla Historial Reproduccion
DELIMITER $$
DROP PROCEDURE IF EXISTS altaHistorial_reproduccion $$
CREATE PROCEDURE altaHistorial_reproduccion (OUT unidHistorial INT UNSIGNED, unidUsuario INT UNSIGNED, unidCancion INT UNSIGNED, unFechaReproduccion DATETIME)
BEGIN 
	INSERT INTO (idUsuario,idCancion,FechaReproduccion)
		VALUES(unidUsuario,unidCancion,unFechaReproduccion)
	
	SET unidHistorial = last_insert_id();
END $$

-- Tabla Playlist
DELIMITER $$
DROP PROCEDURE IF EXISTS IF EXISTS altaPlaylist $$
CREATE PROCEDURE altaPlaylist (unNombre VARCHAR(20), unidUsuario INT UNSIGNED, out unidPlaylist INT UNSIGNED)
BEGIN
	INSERT INTO Playlist(Nombre,idUsuario)
	    VALUES(unNombre,unidUsuario);
	
    SET unidPlaylist = last_insert_id();
END $$

-- Tabla Suscripcion
DELIMITER $$
DROP PROCEDURE IF EXISTS altaSuscripcion $$
CREATE PROCEDURE altaSuscripcion (OUT unidSuscripcion INT UNSIGNED, unaDuracion TINYINT UNSIGNED, unCosto TINYINT UNSIGNED, UntipoSuscripcion VARCHAR(45))
BEGIN
	INSERT INTO Suscripcion (Duracion,Costo,tipoSuscripcion)
		VALUES(unaDuracion,unCosto,UntipoSuscripcion)
	
    SET unidSuscripcion = last_insert_id();
END $$ 