USE 5to_Spotify;

-- Obetener Todos Los atributos de una tabla

--Obtener Artistas
DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerArtistas $$
CREATE PROCEDURE ObtenerArtistas()
BEGIN
	SELECT * 
	from Artista 
	ORDER BY NombreArtistico ASC;
END
$$

--Obtener Canciones
DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerCanciones $$
CREATE PROCEDURE ObtenerCanciones()
BEGIN
	SELECT * 
	from Cancion 
	ORDER BY Titulo ASC;
END
$$

--Obtener Albumes

DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerAlbum $$
CREATE PROCEDURE ObtenerAlbum ()
BEGIN
	SELECT * 
	from Album
	ORDER BY Titulo ASC;
END
$$

--Obtener Generos

DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerGeneros $$
CREATE PROCEDURE ObtenerGeneros ()
BEGIN
	SELECT * 
	from Genero
	ORDER BY Genero ASC;
END
$$

--Obtener Nacionalidades

DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerNacionalidades $$
CREATE PROCEDURE ObtenerNacionalidades ()
BEGIN
	SELECT * 
	from Nacionalidad
	ORDER BY Pais ASC;
END
$$

--Obtener PlayLists

DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerPlayLists $$
CREATE PROCEDURE ObtenerPlayLists ()
BEGIN
	SELECT * 
	from Playlist
	ORDER BY Nombre ASC;
END
$$

--Obtener Reproducciones

DELIMITER $$
DROP PROCEDURE IF EXISTS ObtenerHistorialReproducciones$$
CREATE PROCEDURE ObtenerHistorialReproduccion ()
BEGIN
	SELECT * 
	from HistorialReproducción
	ORDER BY FechaReproduccion ASC;
END
$$

