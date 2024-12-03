USE 5to_Spotify;


-- Obtener Artistas
DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerArtistas $$
CREATE PROCEDURE ObtenerArtistas()
BEGIN
	SELECT * 
	FROM Artista 
	ORDER BY NombreArtistico ASC;
END $$
DELIMITER ;

DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerCanciones $$
CREATE PROCEDURE ObtenerCanciones()
BEGIN
	SELECT * 
	from Cancion 
	ORDER BY Titulo ASC;
END $$

DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerAlbum $$
CREATE PROCEDURE ObtenerAlbum ()
BEGIN
	SELECT * 
	from Album
	ORDER BY Titulo ASC;
END $$


DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerGeneros $$
CREATE PROCEDURE ObtenerGeneros ()
BEGIN
	SELECT * 
	from Genero
	ORDER BY Genero ASC;
END $$

DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerNacionalidades $$
CREATE PROCEDURE ObtenerNacionalidades ()
BEGIN
	SELECT * 
	from Nacionalidad
	ORDER BY Pais ASC;
END $$

DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerPlayLists $$
CREATE PROCEDURE ObtenerPlayLists ()
BEGIN
	SELECT * 
	from Playlist
	ORDER BY Nombre ASC;
END $$
DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerHistorialReproduccion$$
CREATE PROCEDURE ObtenerHistorialReproduccion ()
BEGIN
	SELECT * 
	from HistorialReproduccion
	ORDER BY FechaReproduccion ASC;
END
$$

DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerSuscripciones $$
CREATE PROCEDURE ObtenerSuscripciones ()
BEGIN
	SELECT * 
	from Suscripcion
	ORDER BY FechaInicio ASC;
END $$

DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerTipoSuscripciones $$
CREATE PROCEDURE ObtenerTipoSuscripciones  ()
BEGIN
	SELECT * 
	from TipoSuscripcion
	ORDER BY Tipo ASC;
END $$

DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerUsuarios $$
CREATE PROCEDURE ObtenerUsuarios ()
BEGIN
	SELECT * 
	from Usuario
	ORDER BY NombreUsuario ASC;
END $$

DELIMITER $$
DROP PROCEDURE IF EXISTS BuscarCancionesPorTitulo $$
CREATE PROCEDURE BuscarCancionesPorTitulo(IN unTitulo VARCHAR(45))
BEGIN
	SELECT * 
	FROM Cancion 
	WHERE MATCH(Titulo) AGAINST(unTitulo IN NATURAL LANGUAGE MODE);
END $$
